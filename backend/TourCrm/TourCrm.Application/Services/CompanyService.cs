using TourCrm.Application.Interfaces;
using TourCrm.Core.Entities;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services;

public class CompanyService(
    ICompanyRepository repo,
    IUnitOfWork uow,
    IReferenceDataSeeder referenceDataSeeder
) : ICompanyService
{
    public async Task<Company?> GetMineAsync(string userId, CancellationToken ct = default)
    {
        var byOwner = await repo.GetByOwnerAsync(userId, ct);
        if (byOwner is not null) return byOwner;

        if (!int.TryParse(userId, out var uid)) return null;

        var employee = (await uow.Employees.GetAllAsync(ct))
            .FirstOrDefault(e => e.Id == uid && !e.IsDeleted);
        if (employee is not null)
        {
            var byEmp = await repo.GetByLegalEntityIdAsync(employee.LegalEntityId, ct);
            if (byEmp is not null) return byEmp;
        }

        var roleIds = (await uow.UserRoles.GetAllAsync(ct))
            .Where(ur => ur.UserId == uid)
            .Select(ur => ur.RoleId)
            .ToHashSet();

        if (roleIds.Count > 0)
        {
            var roles = (await uow.Roles.GetAllAsync(ct))
                .Where(r => roleIds.Contains(r.Id) && r.CompanyId.HasValue)
                .ToList();

            var companyId = roles.Select(r => r.CompanyId!.Value).FirstOrDefault();
            if (companyId > 0)
            {
                var anyLe = (await uow.LegalEntities.GetAllAsync(ct))
                    .FirstOrDefault(le => le.CompanyId == companyId && !le.IsDeleted);

                if (anyLe is not null)
                {
                    var byRole = await repo.GetByLegalEntityIdAsync(anyLe.Id, ct);
                    if (byRole is not null) return byRole;
                }
            }
        }

        return null;
    }

    public async Task<Company> CreateAsync(string userId, string name, CancellationToken ct = default)
    {
        if (await repo.ExistsForOwnerAsync(userId, ct))
            throw new InvalidOperationException("У пользователя уже есть компания.");

        var entity = new Company
        {
            Name = name.Trim(),
            OwnerUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repo.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        await referenceDataSeeder.SeedAllAsync(entity.Id, ct);

        if (int.TryParse(userId, out var ownerUserId))
            await referenceDataSeeder.SeedTrialAndDirectorAsync(entity.Id, ownerUserId, ct);

        return entity;
    }

    public async Task<Company> CreateIfMissingAsync(string userId, string name, CancellationToken ct = default)
    {
        var existing = await repo.GetByOwnerAsync(userId, ct);
        if (existing is null)
            return await CreateAsync(userId, name, ct);

        if (int.TryParse(userId, out var ownerUserId))
            await referenceDataSeeder.SeedTrialAndDirectorAsync(existing.Id, ownerUserId, ct);

        return existing;
    }

    public async Task SetMainLegalAsync(string userId, int legalEntityId, CancellationToken ct = default)
    {
        var company = await repo.GetByOwnerAsync(userId, ct)
                      ?? throw new KeyNotFoundException("Компания не найдена.");

        company.LegalEntityId = legalEntityId;
        company.UpdatedAt = DateTime.UtcNow;
        repo.Update(company);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<Company> RenameAsync(string userId, string name, CancellationToken ct = default)
    {
        var company = await repo.GetByOwnerAsync(userId, ct)
                      ?? throw new KeyNotFoundException("Компания не найдена.");

        company.Name = name.Trim();
        company.UpdatedAt = DateTime.UtcNow;
        repo.Update(company);
        await uow.SaveChangesAsync(ct);
        return company;
    }
}