using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly TourCrmDbContext _context;

        public EmployeeRepository(TourCrmDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Office)
                    .ThenInclude(o => o.LegalEntity) 
                .Include(e => e.LegalEntity)
                .Include(e => e.UserRoles).ThenInclude(ur => ur.Role)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.Office)
                    .ThenInclude(o => o.LegalEntity)
                .Include(e => e.LegalEntity)
                .Include(e => e.UserRoles).ThenInclude(ur => ur.Role)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<(IReadOnlyList<Employee> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            int? officeId = null,
            bool? isDeleted = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.Employees
                .Include(e => e.Office)
                    .ThenInclude(o => o.LegalEntity)
                .Include(e => e.LegalEntity)
                .Include(e => e.UserRoles).ThenInclude(ur => ur.Role)
                .AsNoTracking()
                .AsSplitQuery()
                .AsQueryable();

            if (officeId.HasValue)
                query = query.Where(e => e.OfficeId == officeId.Value);

            if (isDeleted.HasValue)
                query = query.Where(e => e.IsDeleted == isDeleted.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(e => e.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<IReadOnlyList<Employee>> GetByOfficeAsync(int officeId)
        {
            return await _context.Employees
                .Where(e => e.OfficeId == officeId)
                .Include(e => e.Office)
                    .ThenInclude(o => o.LegalEntity)
                .Include(e => e.LegalEntity)
                .Include(e => e.UserRoles).ThenInclude(ur => ur.Role)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();
        }
    }
}



