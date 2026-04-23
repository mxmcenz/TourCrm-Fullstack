using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories.Dictionaries;

public class NumberTypeRepository(TourCrmDbContext context) : GenericRepository<NumberType>(context),  INumberTypeRepository;