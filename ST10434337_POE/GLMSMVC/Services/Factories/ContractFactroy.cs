using GLMSMVC.Models.Domain;
using GLMSMVC.Models.DTOs;
using GLMSMVC.Models.Enums;
using GLMSMVC.Services.Interfaces;

namespace GLMSMVC.Services.Factories
{
    public class ContractFactory : IContractFactory //(Geekific, 2021a)
    {
        public Contract Create(CreateContractDTO dto, string? filePath = null)
        {
            return new Contract
            {
                ClientId = dto.ClientId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ServiceLevel = dto.ServiceLevel,
                FilePath = filePath,
                Status = ContractStatus.Draft,
                IsPaused = false
            };
        }
    }
}
/*
 
 Geekific. 2021a. The Factory Method Pattern Explained and Implemented in Java | 
Creational Design Patterns. [online] Available at: 
<https://www.youtube.com/watch?v=EdFq_JIThqM> [Accessed 28 March 2026]. 
 */