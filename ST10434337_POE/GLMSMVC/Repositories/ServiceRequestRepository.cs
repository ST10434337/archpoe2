using GLMSMVC.Data;
using GLMSMVC.Models.Domain;
using GLMSMVC.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GLMSMVC.Repositories
{
    // Database Access for Service Request
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllAsync()
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .ToListAsync();
        }

        public async Task<ServiceRequest?> GetByIdAsync(int id)
        {
            return await _context.ServiceRequests
                .FirstOrDefaultAsync(sr => sr.Id == id);
        }

        public async Task<ServiceRequest?> GetByContractIdAsync(int contractId)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(sr => sr.ContractId == contractId);
        }

        public async Task<ServiceRequest?> GetWithContractAsync(int id)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(sr => sr.Id == id);
        }

        public async Task AddAsync(ServiceRequest serviceRequest)
        {
            await _context.ServiceRequests.AddAsync(serviceRequest);
        }

        public void Update(ServiceRequest serviceRequest)
        {
            _context.ServiceRequests.Update(serviceRequest);
        }

        // May not implement in my buisness logic but is nice to have
        public void Delete(ServiceRequest serviceRequest)
        {
            _context.ServiceRequests.Remove(serviceRequest);
        }

        // Checks if SR is in Database
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ServiceRequests.AnyAsync(sr => sr.Id == id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

