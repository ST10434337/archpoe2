using GLMSMVC.Models.Domain;
using GLMSMVC.Models.Enums;

namespace GLMSMVC.Repositories.Interfaces
{
    public interface IContractRepository
    {
        Task<IEnumerable<Contract>> GetAllAsync();
        Task<Contract?> GetByIdAsync(int id);
        Task<Contract?> GetWithClientAsync(int id);
        Task<Contract?> GetWithServiceRequestAsync(int id);
        Task<IEnumerable<Contract>> GetFilteredAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status);
        Task AddAsync(Contract contract);
        void Update(Contract contract);
        void Delete(Contract contract); // May not implement in Buisness Logic
        Task<bool> ExistsAsync(int id);
        Task<int> SaveChangesAsync();
    }
}
