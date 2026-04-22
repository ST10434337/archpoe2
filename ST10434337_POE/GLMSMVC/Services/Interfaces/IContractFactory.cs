using GLMSMVC.Models.Domain;
using GLMSMVC.Models.DTOs;

namespace GLMSMVC.Services.Interfaces
{
    public interface IContractFactory
    {
        Contract Create(CreateContractDTO dto, string? filePath);
    }
}
