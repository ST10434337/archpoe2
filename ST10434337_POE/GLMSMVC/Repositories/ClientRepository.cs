using GLMSMVC.Data;
using GLMSMVC.Models.Domain;
using GLMSMVC.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GLMSMVC.Repositories
{
    // Database Access for Client 
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await _context.Clients
                .Include(c => c.Contracts)
                .ToListAsync();
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.Contracts)
                .ThenInclude(c => c.ServiceRequest)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Client client)
        {
            await _context.Clients.AddAsync(client);
        }

        public void Update(Client client)
        {
            _context.Clients.Update(client);
        }

        public void Delete(Client client)
        {
            _context.Clients.Remove(client);
        }

        public async Task<bool> ExistsAsync(int id)// Check if record Exists
        {
            return await _context.Clients.AnyAsync(c => c.Id == id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
