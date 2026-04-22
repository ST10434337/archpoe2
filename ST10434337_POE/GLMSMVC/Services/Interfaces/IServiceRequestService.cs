using GLMSMVC.Models.Domain;
using GLMSMVC.Models.DTOs;
using GLMSMVC.Models.Enums;
using GLMSMVC.Services.Results;

namespace GLMSMVC.Services.Interfaces
{
    public interface IServiceRequestService
    {
        Task<ServiceResult<IEnumerable<ServiceRequest>>> GetAllAsync();
        Task<ServiceResult<ServiceRequest>> GetByIdAsync(int id);
        Task<ServiceResult<ServiceRequest>> CreateAsync(int contractId, string description, decimal originalAmount, string currencyCode);

        Task<ServiceResult<ServiceRequest>> UpdateAsync(
            int id,
            string description,
            decimal originalAmount,
            string currencyCode,
            ServiceRequestStatus status,
            bool useApi);
    }
}
