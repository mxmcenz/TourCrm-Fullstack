namespace TourCrm.Application.Interfaces;

public interface IReferenceDataSeeder
{
    Task SeedAllAsync(int companyId, CancellationToken ct = default);
    Task SeedTrialAndDirectorAsync(int companyId, int ownerUserId, CancellationToken ct = default);
}