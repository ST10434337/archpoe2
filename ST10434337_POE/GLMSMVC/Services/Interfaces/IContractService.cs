using GLMSMVC.Models.Domain;
using GLMSMVC.Models.DTOs;
using GLMSMVC.Models.Enums;
using GLMSMVC.Services.Results;

namespace GLMSMVC.Services.Interfaces
{
    public interface IContractService
    {
        Task<ServiceResult<IEnumerable<Contract>>> GetAllAsync();
        Task<ServiceResult<Contract>> GetByIdAsync(int id);
        Task<ServiceResult<IEnumerable<Contract>>> GetFilteredAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status);
        Task<ServiceResult<Contract>> CreateAsync(CreateContractDTO dto, IFormFile? file);
        Task<ServiceResult<Contract>> UpdateAsync(int id, CreateContractDTO dto, IFormFile? file, bool pauseContract);
    }
}
