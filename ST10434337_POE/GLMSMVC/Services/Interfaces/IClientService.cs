using GLMSMVC.Models.Domain;
using GLMSMVC.Models.DTOs;
using GLMSMVC.Services.Results;

namespace GLMSMVC.Services.Interfaces
{
    public interface IClientService
    {
        Task<ServiceResult<IEnumerable<Client>>> GetAllAsync();
        Task<ServiceResult<Client>> GetByIdAsync(int id);
        Task<ServiceResult<Client>> CreateAsync(Client client);
        Task<ServiceResult<Client>> UpdateAsync(Client client);
    }
}
