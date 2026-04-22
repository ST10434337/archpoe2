using GLMSMVC.Models.Domain;

namespace GLMSMVC.Services.Interfaces
{
    public interface IServiceRequestFactory
    {
        ServiceRequest CreateFromContract(Contract contract, string description, decimal originalAmount = 0,
            string sourceCurrency = "USD", decimal costZar = 0, bool usedApi = false);
    }
}
