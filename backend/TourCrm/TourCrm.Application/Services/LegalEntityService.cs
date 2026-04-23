using TourCrm.Application.DTOs.LegalEntity;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Entities;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services;

public class LegalEntityService(
    ILegalEntityRepository leRepo,
    ICompanyRepository companyRepo,
    IUnitOfWork uow
) : ILegalEntityService
{
    private const string SuperAdminRoleName = "SuperAdmin";

    private async Task<bool> IsSuperAdminAsync(string userId, CancellationToken ct)
    {
        if (!int.TryParse(userId, out var uid)) return false;
        var userRoles = (await uow.UserRoles.GetAllAsync(ct)).ToList();
        if (userRoles.Count == 0) return false;

        var roleIds = userRoles.Where(ur => ur.UserId == uid).Select(ur => ur.RoleId).ToHashSet();
        if (roleIds.Count == 0) return false;

        var roles = (await uow.Roles.GetAllAsync(ct)).ToList();
        return roles.Any(r =>
            roleIds.Contains(r.Id) && r.Name.Equals(SuperAdminRoleName, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<int> GetMyCompanyIdOrThrowAsync(string userId, CancellationToken ct)
    {
        var company = await companyRepo.GetByOwnerAsync(userId, ct);
        if (company is not null) return company.Id;

        if (!int.TryParse(userId, out var uid))
            throw new InvalidOperationException("Некорректный идентификатор пользователя.");

        var me = await uow.Employees.GetByIdAsync(uid);
        if (me is null)
            throw new UnauthorizedAccessException("Пользователь не найден.");
        if (me.LegalEntityId <= 0)
            throw new InvalidOperationException("Пользователь не привязан к юрлицу.");

        var companyByLe = await companyRepo.GetByLegalEntityIdAsync(me.LegalEntityId, ct)
                          ?? throw new InvalidOperationException("Компания для указанного юрлица не найдена.");

        return companyByLe.Id;
    }

    public async Task<IReadOnlyList<LegalEntityListItemDto>> GetMineAsync(string userId, string? q = null,
        CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);

        List<LegalEntity> list = isSa
            ? (await leRepo.GetAllAsync(ct)).Where(x => !x.IsDeleted).ToList()
            : (await leRepo.GetByCompanyAsync(await GetMyCompanyIdOrThrowAsync(userId, ct), ct))
              .Where(x => !x.IsDeleted).ToList();

        static List<string>? ParsePhones(string? phones) =>
            string.IsNullOrWhiteSpace(phones)
                ? null
                : phones.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        var projected = list.Select(x =>
        {
            var phonesList = ParsePhones(x.Phones);
            return new LegalEntityListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                CountryId = x.CountryId,
                CountryName = x.CountryRef?.Name,
                CityId = x.CityId,
                CityName = x.CityRef?.Name,
                Phone = phonesList?.FirstOrDefault(),
                Phones = phonesList,
                PrimaryEmail = x.Email,
                EmployeesCount = x.Offices?.Sum(o => o.Employees?.Count ?? 0) ?? 0,
                CreatedAt = x.CreatedAt
            };
        }).ToList();

        if (string.IsNullOrWhiteSpace(q))
            return projected.OrderBy(x => x.Name).ToList();

        var qTrim = q.Trim();
        var needle = qTrim.ToLowerInvariant();
        var onlyDigits = new string(qTrim.Where(char.IsDigit).ToArray());
        int? empFilter = (onlyDigits.Length == qTrim.Length && int.TryParse(onlyDigits, out var n)) ? n : null;

        static string Digits(string? s) => new((s ?? string.Empty).Where(char.IsDigit).ToArray());

        var filtered = projected.Where(x =>
            (!string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Contains(needle)) ||
            (!string.IsNullOrEmpty(x.CountryName) && x.CountryName!.ToLower().Contains(needle)) ||
            (!string.IsNullOrEmpty(x.CityName) && x.CityName!.ToLower().Contains(needle)) ||
            (!string.IsNullOrEmpty(x.PrimaryEmail) && x.PrimaryEmail!.ToLower().Contains(needle)) ||
            (!string.IsNullOrEmpty(x.Phone) && (x.Phone!.ToLower().Contains(needle) ||
                                                (!string.IsNullOrEmpty(onlyDigits) && Digits(x.Phone).Contains(onlyDigits)))) ||
            (x.Phones != null && x.Phones.Any(p => p.ToLower().Contains(needle) ||
                                                   (!string.IsNullOrEmpty(onlyDigits) && Digits(p).Contains(onlyDigits)))) ||
            (empFilter.HasValue && x.EmployeesCount == empFilter.Value)
        );

        return filtered.OrderBy(x => x.Name).ToList();
    }

    public async Task<LegalEntityDto?> GetAsync(int id, string userId, CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);
        var e = await leRepo.GetByIdAsync(id, ct);
        if (e is null || e.IsDeleted) return null;

        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
            if (e.CompanyId != companyId) return null;
        }

        return ToDto(e);
    }

    public async Task<LegalEntityDto> CreateAsync(LegalEntityUpsertDto dto, string userId,
        CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);

        int companyId;
        if (isSa)
        {
            if (dto.CompanyId is null || dto.CompanyId <= 0)
                throw new InvalidOperationException("Не указан CompanyId.");
            companyId = dto.CompanyId.Value;
        }
        else
        {
            companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
        }

        var displayName = (dto.NameRu ?? dto.NameFull ?? dto.NameEn ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(displayName))
            throw new InvalidOperationException("Название (рус.) или Полное название обязательно.");

        if (await leRepo.ExistsByNameInCompanyAsync(companyId, displayName, ct))
            throw new InvalidOperationException("Юрлицо с таким названием уже существует в указанной компании.");

        var e = new LegalEntity
        {
            CompanyId = companyId,
            Name = displayName,
            NameRu = dto.NameRu,
            NameEn = dto.NameEn,
            NameFull = dto.NameFull,

            CountryId = dto.CountryId,
            CityId = dto.CityId,

            LegalAddress = dto.LegalAddress,
            ActualAddress = dto.ActualAddress,

            DirectorFio = dto.DirectorFio,
            DirectorFioGen = dto.DirectorFioGen,
            DirectorPost = dto.DirectorPost,
            DirectorPostGen = dto.DirectorPostGen,
            DirectorBasis = dto.DirectorBasis,

            CfoFio = dto.CfoFio,
            Phones = dto.Phones,
            Website = dto.Website,
            Email = dto.Email,
            BinIin = dto.BinIin,
            BankDetailsJson = dto.BankDetailsJson,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await leRepo.AddAsync(e, ct);
        await uow.SaveChangesAsync(ct);

        return ToDto(e);
    }

    public async Task<LegalEntityDto> UpdateAsync(int id, LegalEntityUpsertDto dto, string userId,
        CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);
        var e = await leRepo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Юрлицо не найдено");

        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
            if (e.CompanyId != companyId) throw new UnauthorizedAccessException("Нет доступа к юрлицу.");
        }

        var displayName = (dto.NameRu ?? dto.NameFull ?? dto.NameEn ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(displayName))
            throw new InvalidOperationException("Название (рус.) или Полное название обязательно.");

        var targetCompanyId = e.CompanyId;
        if (!string.Equals(e.Name, displayName, StringComparison.OrdinalIgnoreCase) &&
            await leRepo.ExistsByNameInCompanyAsync(targetCompanyId, displayName, ct))
            throw new InvalidOperationException("Юрлицо с таким названием уже существует в компании.");

        e.Name = displayName;
        e.NameRu = dto.NameRu;
        e.NameEn = dto.NameEn;
        e.NameFull = dto.NameFull;

        e.CountryId = dto.CountryId;
        e.CityId = dto.CityId;

        e.LegalAddress = dto.LegalAddress;
        e.ActualAddress = dto.ActualAddress;

        e.DirectorFio = dto.DirectorFio;
        e.DirectorFioGen = dto.DirectorFioGen;
        e.DirectorPost = dto.DirectorPost;
        e.DirectorPostGen = dto.DirectorPostGen;
        e.DirectorBasis = dto.DirectorBasis;

        e.CfoFio = dto.CfoFio;
        e.Phones = dto.Phones;
        e.Website = dto.Website;
        e.Email = dto.Email;
        e.BinIin = dto.BinIin;
        e.BankDetailsJson = dto.BankDetailsJson;

        e.UpdatedAt = DateTime.UtcNow;

        leRepo.Update(e);
        await uow.SaveChangesAsync(ct);
        return ToDto(e);
    }

    public async Task SoftDeleteAsync(int id, string userId, CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);
        var e = await leRepo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Юрлицо не найдено");

        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
            if (e.CompanyId != companyId) throw new UnauthorizedAccessException("Нет доступа к юрлицу.");
        }

        e.IsDeleted = true;
        e.UpdatedAt = DateTime.UtcNow;

        leRepo.Update(e);
        await uow.SaveChangesAsync(ct);
    }

    private static LegalEntityDto ToDto(LegalEntity e) => new()
    {
        Id = e.Id,

        Name = e.Name,
        NameRu = e.NameRu,
        NameEn = e.NameEn,
        NameFull = e.NameFull,

        CountryId = e.CountryId,
        CountryName = e.CountryRef?.Name,
        CityId = e.CityId,
        CityName = e.CityRef?.Name,

        LegalAddress = e.LegalAddress,
        ActualAddress = e.ActualAddress,

        DirectorFio = e.DirectorFio,
        DirectorFioGen = e.DirectorFioGen,
        DirectorPost = e.DirectorPost,
        DirectorPostGen = e.DirectorPostGen,
        DirectorBasis = e.DirectorBasis,

        CfoFio = e.CfoFio,
        Phones = e.Phones,
        Website = e.Website,
        Email = e.Email,
        BinIin = e.BinIin,
        BankDetailsJson = e.BankDetailsJson,

        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}