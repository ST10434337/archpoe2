using GLMSMVC.Models.Domain;

namespace GLMSMVC.Repositories.Interfaces
{
    public interface IServiceRequestRepository
    {
        Task<IEnumerable<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(int id);
        Task<ServiceRequest?> GetByContractIdAsync(int contractId);
        Task<ServiceRequest?> GetWithContractAsync(int id);
        Task AddAsync(ServiceRequest serviceRequest);
        void Update(ServiceRequest serviceRequest);
        void Delete(ServiceRequest serviceRequest);// May not implement in Buisness Logic
        Task<bool> ExistsAsync(int id);
        Task<int> SaveChangesAsync();
    }
}
