using TourCrm.Core.Entities;

namespace TourCrm.Core.Interfaces;

public interface ILegalEntityRepository : IGenericRepository<LegalEntity>
{
    Task<bool> ExistsByNameInCompanyAsync(int companyId, string name, CancellationToken ct = default);
    Task<IReadOnlyList<LegalEntity>> GetByCompanyAsync(int companyId, CancellationToken ct = default); 
}