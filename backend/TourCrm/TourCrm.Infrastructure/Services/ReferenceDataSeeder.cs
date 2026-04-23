using Microsoft.Extensions.Options;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Permissions;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Application.Settings;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Roles;
using TourCrm.Core.Entities.Tariffs;
using TourCrm.Core.Interfaces;

namespace TourCrm.Infrastructure.Services;

public class ReferenceDataSeeder(
    IMealTypeService mealTypeService,
    IPartnerTypeService partnerTypeService,
    IServiceTypeService serviceTypeService,
    IAccommodationTypeService accommodationTypeService,
    IUnitOfWork uow,
    IPermissionProvider permProvider,
    IOptions<TrialSettings> trial
) : IReferenceDataSeeder
{
    public async Task SeedAllAsync(int companyId, CancellationToken ct = default)
    {
        await mealTypeService.SeedDefaultsForCompanyAsync(companyId, ct);
        await partnerTypeService.SeedDefaultsForCompanyAsync(companyId, ct);
        await serviceTypeService.SeedDefaultsForCompanyAsync(companyId, ct);
        await accommodationTypeService.SeedDefaultsForCompanyAsync(companyId, ct);
    }

    public async Task SeedTrialAndDirectorAsync(int companyId, int ownerUserId, CancellationToken ct = default)
    {
        var allPerms = await permProvider.GetPermissionsAsync();

        var trialTariff = await FindTariffByNameAsync(trial.Value.TariffName, ct)
                          ?? await CreateTrialTariffAsync(allPerms, ct);

        await uow.TariffPermissions.ReplaceForTariffAsync(
            trialTariff.Id,
            allPerms.Select(p => new TariffPermission
            {
                TariffId = trialTariff.Id,
                PermissionKey = p.Key,
                IsGranted = true
            }),
            ct);
        await uow.SaveChangesAsync(ct);

        var company = await uow.Companies.GetByIdAsync(companyId, ct)
                      ?? throw new KeyNotFoundException("Company not found");

        if (company.TariffId is null)
        {
            company.TariffId = trialTariff.Id;
            company.TariffExpiresAt = DateTime.UtcNow.AddDays(trial.Value.Days);
            uow.Companies.Update(company);
            await uow.SaveChangesAsync(ct);
        }

        var director = await FindRoleByNameAsync("Директор", companyId, ct)
                       ?? await CreateDirectorRoleAsync(companyId, ct);

        var grantedByTariff = (await uow.TariffPermissions.GetByTariffIdAsync(trialTariff.Id, ct))
            .Where(x => x.IsGranted)
            .Select(x => x.PermissionKey)
            .ToHashSet(StringComparer.Ordinal);

        await uow.RolePermissions.ReplaceForRoleAsync(
            director.Id,
            allPerms.Select(p => new RolePermission
            {
                RoleId = director.Id,
                PermissionKey = p.Key,
                IsGranted = grantedByTariff.Contains(p.Key)
            }),
            ct);
        await uow.SaveChangesAsync(ct);

        if (!await uow.UserRoles.ExistsAsync(ownerUserId, director.Id))
        {
            await uow.UserRoles.AddAsync(new UserRole { UserId = ownerUserId, RoleId = director.Id }, ct);
            await uow.SaveChangesAsync(ct);
        }
    }

    private async Task<Tariff?> FindTariffByNameAsync(string name, CancellationToken ct)
    {
        var all = await uow.Tariffs.GetAllLightAsync(ct);
        return all.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<Role?> FindRoleByNameAsync(string name, int companyId, CancellationToken ct)
    {
        var all = await uow.Roles.GetAllAsync(ct);
        return all.FirstOrDefault(r =>
            r.CompanyId == companyId && r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<Tariff> CreateTrialTariffAsync(IReadOnlyList<PermissionDto> allPerms, CancellationToken ct)
    {
        var t = new Tariff
        {
            Name = trial.Value.TariffName,
            MinEmployees = trial.Value.MinEmployees,
            MaxEmployees = trial.Value.MaxEmployees,
            MonthlyPrice = 0,
            HalfYearPrice = 0,
            YearlyPrice = 0,
            IsPublic = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await uow.Tariffs.AddAsync(t, ct);
        await uow.SaveChangesAsync(ct);

        await uow.TariffPermissions.ReplaceForTariffAsync(
            t.Id,
            allPerms.Select(p => new TariffPermission { TariffId = t.Id, PermissionKey = p.Key, IsGranted = true }),
            ct);
        await uow.SaveChangesAsync(ct);

        return t;
    }

    private async Task<Role> CreateDirectorRoleAsync(int companyId, CancellationToken ct)
    {
        var role = new Role { Name = "Директор", CompanyId = companyId };
        await uow.Roles.AddAsync(role, ct);
        await uow.SaveChangesAsync(ct);
        return role;
    }
}
