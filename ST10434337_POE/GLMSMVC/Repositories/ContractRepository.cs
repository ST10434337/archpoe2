using GLMSMVC.Data;
using GLMSMVC.Models.Domain;
using GLMSMVC.Models.Enums;
using GLMSMVC.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GLMSMVC.Repositories
{
    // Database Access for Contract
    public class ContractRepository : IContractRepository
    {
        private readonly ApplicationDbContext _context;

        public ContractRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contract>> GetAllAsync()
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ServiceRequest)
                .ToListAsync();
        }

        public async Task<Contract?> GetByIdAsync(int id)
        {
            return await _context.Contracts
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Contract?> GetWithClientAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Contract?> GetWithServiceRequestAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ServiceRequest)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // For Contract Filter/Search on Index, fix Range search if any in any of that range it will now show
        public async Task<IEnumerable<Contract>> GetFilteredAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            IQueryable<Contract> query = _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ServiceRequest);

            // If both dates are provided
            if (startDate.HasValue && endDate.HasValue)
            {
                var start = startDate.Value.Date;
                var end = endDate.Value.Date;

                // Return contracts that overlap with selected range
                query = query.Where(c =>
                    c.StartDate.HasValue &&
                    c.EndDate.HasValue &&
                    c.StartDate.Value.Date <= end &&
                    c.EndDate.Value.Date >= start);
            }
            // If only Start date provided
            else if (startDate.HasValue)
            {
                var start = startDate.Value.Date;
                
                // Return contracts that are still active after start date
                query = query.Where(c =>
                    c.EndDate.HasValue &&
                    c.EndDate.Value.Date >= start);
            }
            // If only End date provided
            else if (endDate.HasValue)
            {
                var end = endDate.Value.Date;

                // Return contracts that started before given end date
                query = query.Where(c =>
                    c.StartDate.HasValue &&
                    c.StartDate.Value.Date <= end);
            }

            // Filter by contract Status if provided
            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            // Sort by client name
            return await query
                .OrderBy(c => c.Client!.Name)
                .ThenBy(c => c.StartDate)
                .ToListAsync();
        }

        public async Task AddAsync(Contract contract)
        {
            await _context.Contracts.AddAsync(contract);
        }

        public void Update(Contract contract)
        {
            _context.Contracts.Update(contract);
        }

        public void Delete(Contract contract)// May not implement in buisness logic
        {
            _context.Contracts.Remove(contract);
        }

        public async Task<bool> ExistsAsync(int id) // Does contract already exist
        {
            return await _context.Contracts.AnyAsync(c => c.Id == id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
