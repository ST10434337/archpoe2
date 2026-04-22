using GLMSMVC.Models.DTOs;
using GLMSMVC.Services.Results;

namespace GLMSMVC.Services.Interfaces
{
    public interface IContractFileService
    {
        Task<ServiceResult<string>> UploadFileAsync(int contractId, IFormFile? file);
        Task<ServiceResult<FileDownloadResultDTO>> DownloadFileAsync(int contractId);
    }
}
