using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories.Dictionaries;

public class TourOperatorRepository (TourCrmDbContext context) : GenericRepository<TourOperator>(context), ITourOperatorRepository;
