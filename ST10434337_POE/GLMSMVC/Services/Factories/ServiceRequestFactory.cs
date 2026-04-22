using GLMSMVC.Models.Domain;
using GLMSMVC.Models.Enums;
using GLMSMVC.Services.Interfaces;

namespace GLMSMVC.Services.Factories
{
    public class ServiceRequestFactory : IServiceRequestFactory //(Geekific, 2021a)
    {
        public ServiceRequest CreateFromContract(Contract contract, string description, decimal originalAmount = 0, 
            string sourceCurrency = "USD", decimal costZar = 0, bool usedApi = false)
        {
            return new ServiceRequest
            {
                ContractId = contract.Id,
                Contract = contract,
                Description = description,
                OriginalAmount = originalAmount,
                SourceCurrency = sourceCurrency.ToUpperInvariant(),
                CostZar = costZar,
                Status = ServiceRequestStatus.Pending,
                UsedAPI = usedApi
            };
        }
    }
}
/*
 
 Geekific. 2021a. The Factory Method Pattern Explained and Implemented in Java | 
Creational Design Patterns. [online] Available at: 
<https://www.youtube.com/watch?v=EdFq_JIThqM> [Accessed 28 March 2026]. 
 */