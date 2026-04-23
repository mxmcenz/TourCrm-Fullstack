using TourCrm.Core.Entities;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public class PartnerRepository(TourCrmDbContext context) : GenericRepository<Partner>(context), IPartnerRepository;