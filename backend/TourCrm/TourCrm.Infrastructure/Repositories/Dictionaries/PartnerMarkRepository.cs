using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories.Dictionaries;

public class PartnerMarkRepository(TourCrmDbContext context) : GenericRepository<PartnerMark>(context), IPartnerMarkRepository;