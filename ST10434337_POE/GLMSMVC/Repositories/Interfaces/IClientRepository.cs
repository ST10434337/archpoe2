using GLMSMVC.Models.Domain;

namespace GLMSMVC.Repositories.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int id);
        Task AddAsync(Client client);
        void Update(Client client);
        void Delete(Client client);
        Task<bool> ExistsAsync(int id);
        Task<int> SaveChangesAsync();
    }
}
