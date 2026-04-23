using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs.Tariffs;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Entities.Tariffs;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services;

public class TariffService(
    IUnitOfWork uow,
    IPermissionProvider permissionProvider
) : ITariffService
{
    public async Task<ServiceResult<TariffDto>> CreateAsync(CreateTariffDto dto, CancellationToken ct = default)
    {
        var vr = Validate(dto);
        if (vr is not null) return ServiceResult<TariffDto>.Fail(vr);

        if (await uow.Tariffs.NameExistsAsync(dto.Name.Trim(), null, ct))
            return ServiceResult<TariffDto>.Fail("Тариф с таким названием уже существует");

        var allowed = await permissionProvider.GetPermissionsAsync();
        var allowedSet = allowed.Select(p => p.Key).ToHashSet(StringComparer.Ordinal);

        if (dto.Permissions.Any(p => !allowedSet.Contains(p.PermissionKey)))
            return ServiceResult<TariffDto>.Fail("Содержатся несуществующие ключи доступов");

        var t = new Tariff
        {
            Name = dto.Name.Trim(),
            MinEmployees = dto.MinEmployees,
            MaxEmployees = dto.MaxEmployees,
            MonthlyPrice = dto.MonthlyPrice,
            HalfYearPrice = dto.HalfYearPrice,
            YearlyPrice = dto.YearlyPrice,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await uow.Tariffs.AddAsync(t, ct);
        await uow.SaveChangesAsync(ct);

        var requested = dto.Permissions.ToDictionary(x => x.PermissionKey, x => x.IsGranted, StringComparer.Ordinal);
        var items = allowed.Select(a => new TariffPermission
        {
            TariffId = t.Id,
            PermissionKey = a.Key,
            IsGranted = requested.TryGetValue(a.Key, out var g) && g
        });

        await uow.TariffPermissions.ReplaceForTariffAsync(t.Id, items, ct);
        await uow.SaveChangesAsync(ct);

        var fullDtoPerms = allowed
            .Select(a => new TariffPermissionDto(a.Key, requested.TryGetValue(a.Key, out var g) && g))
            .ToList();

        return ServiceResult<TariffDto>.Ok(Map(t, fullDtoPerms), "Тариф создан");
    }

    public async Task<ServiceResult<TariffDto>> UpdateAsync(int id, UpdateTariffDto dto, CancellationToken ct = default)
    {
        var t = await uow.Tariffs.GetWithPermissionsAsync(id, ct);
        if (t is null) return ServiceResult<TariffDto>.Fail("Тариф не найден");

        var vr = Validate(dto);
        if (vr is not null) return ServiceResult<TariffDto>.Fail(vr);

        if (await uow.Tariffs.NameExistsAsync(dto.Name.Trim(), id, ct))
            return ServiceResult<TariffDto>.Fail("Тариф с таким названием уже существует");

        t.Name = dto.Name.Trim();
        t.MinEmployees = dto.MinEmployees;
        t.MaxEmployees = dto.MaxEmployees;
        t.MonthlyPrice = dto.MonthlyPrice;
        t.HalfYearPrice = dto.HalfYearPrice;
        t.YearlyPrice = dto.YearlyPrice;
        t.UpdatedAt = DateTime.UtcNow;

        var allowedSet = (await permissionProvider.GetPermissionsAsync())
            .Select(p => p.Key).ToHashSet(StringComparer.Ordinal);

        var requested = dto.Permissions.ToDictionary(p => p.PermissionKey, p => p.IsGranted, StringComparer.Ordinal);
        var normalized = allowedSet.Select(k => new TariffPermission
        {
            TariffId = id,
            PermissionKey = k,
            IsGranted = requested.TryGetValue(k, out var g) && g
        });

        await uow.TariffPermissions.ReplaceForTariffAsync(id, normalized, ct);
        await uow.SaveChangesAsync(ct);

        var x = await uow.Tariffs.GetWithPermissionsAsync(id, ct);
        var dtoOut = new TariffDto(
            x!.Id, x.Name, x.MinEmployees, x.MaxEmployees,
            x.MonthlyPrice, x.HalfYearPrice, x.YearlyPrice,
            x.Permissions
                .OrderBy(p => p.PermissionKey, StringComparer.Ordinal)
                .Select(p => new TariffPermissionDto(p.PermissionKey, p.IsGranted))
                .ToList()
        );

        return ServiceResult<TariffDto>.Ok(dtoOut, "Тариф обновлён");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var t = await uow.Tariffs.GetByIdAsync(id, ct);
        if (t is null) return ServiceResult<object>.Ok("Уже удалён");

        var anyCompany = (await uow.Companies.GetAllAsync(ct)).Any(c => c.TariffId == id);
        if (anyCompany) return ServiceResult<object>.Fail("Есть компании на этом тарифе");

        uow.Tariffs.Delete(t);
        await uow.SaveChangesAsync(ct);
        return ServiceResult<object>.Ok("Тариф удалён");
    }

    public async Task<ServiceResult<TariffDto?>> GetAsync(int id, CancellationToken ct = default)
    {
        var t = await uow.Tariffs.GetWithPermissionsAsync(id, ct);
        if (t is null || !t.IsPublic)
            return ServiceResult<TariffDto?>.Ok(null!);

        var allowed = await permissionProvider.GetPermissionsAsync();
        var grantedMap = t.Permissions.ToDictionary(p => p.PermissionKey, p => p.IsGranted, StringComparer.Ordinal);

        var perms = allowed
            .Select(a => new TariffPermissionDto(a.Key, grantedMap.TryGetValue(a.Key, out var g) && g))
            .ToList();

        return ServiceResult<TariffDto?>.Ok(Map(t, perms));
    }

    public async Task<ServiceResult<List<TariffDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var all = await uow.Tariffs.GetAllLightAsync(ct);
        var list = all
            .Where(t => t.IsPublic)
            .Select(t => Map(t, Array.Empty<TariffPermissionDto>()))
            .ToList();
        return ServiceResult<List<TariffDto>>.Ok(list);
    }

    public async Task<ServiceResult<TariffDto?>> SuggestForEmployeesAsync(int employees, CancellationToken ct = default)
    {
        if (employees < 0) return ServiceResult<TariffDto?>.Fail("Число сотрудников некорректно");

        var t = (await uow.Tariffs.GetAllLightAsync(ct))
            .Where(x => x.IsPublic && x.MinEmployees <= employees && employees <= x.MaxEmployees)
            .OrderBy(x => x.MonthlyPrice)
            .FirstOrDefault();

        return ServiceResult<TariffDto?>.Ok(t is null ? null : Map(t, Array.Empty<TariffPermissionDto>()));
    }

    public Task<ServiceResult<object>> AssignToCompanyAsync(int companyId, int tariffId, CancellationToken ct = default)
        => ChangeCompanyTariffAsync(companyId, tariffId, ct);

    public async Task<ServiceResult<object>> ChangeCompanyTariffAsync(int companyId, int tariffId,
        CancellationToken ct = default)
    {
        var company = await uow.Companies.GetByIdAsync(companyId, ct);
        if (company is null) return ServiceResult<object>.Fail("Компания не найдена");

        var tariff = await uow.Tariffs.GetByIdAsync(tariffId, ct);
        if (tariff is null) return ServiceResult<object>.Fail("Тариф не найден");

        company.TariffId = tariffId;
        uow.Companies.Update(company);
        await uow.SaveChangesAsync(ct);
        return ServiceResult<object>.Ok("Тариф привязан к компании");
    }

    public async Task<ServiceResult<object>> RemoveCompanyTariffAsync(int companyId, CancellationToken ct = default)
    {
        var company = await uow.Companies.GetByIdAsync(companyId, ct);
        if (company is null) return ServiceResult<object>.Fail("Компания не найдена");

        company.TariffId = null;
        uow.Companies.Update(company);
        await uow.SaveChangesAsync(ct);

        return ServiceResult<object>.Ok("Тариф отвязан от компании");
    }

    static string? Validate(CreateTariffDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) return "Название обязательно";
        if (dto.MinEmployees < 0 || dto.MaxEmployees < 0) return "Штат не может быть отрицательным";
        if (dto.MinEmployees > dto.MaxEmployees) return "MinEmployees > MaxEmployees";
        if (dto.MonthlyPrice < 0 || (dto.HalfYearPrice is < 0) || (dto.YearlyPrice is < 0))
            return "Цена не может быть отрицательной";
        return null;
    }

    static string? Validate(UpdateTariffDto dto) => Validate(new CreateTariffDto(
        dto.Name, dto.MinEmployees, dto.MaxEmployees, dto.MonthlyPrice, dto.HalfYearPrice, dto.YearlyPrice,
        dto.Permissions));

    static TariffDto Map(Tariff t, IReadOnlyList<TariffPermissionDto> perms) => new(
        t.Id, t.Name, t.MinEmployees, t.MaxEmployees, t.MonthlyPrice, t.HalfYearPrice, t.YearlyPrice, perms);
}