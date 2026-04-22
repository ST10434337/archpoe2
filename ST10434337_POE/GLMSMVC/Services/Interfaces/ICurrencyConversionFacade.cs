using GLMSMVC.Services.Results;

namespace GLMSMVC.Services.Interfaces
{
    public interface ICurrencyConversionFacade //(Geekific, 2022)
    {
        Task<ServiceResult<decimal>> ConvertCostToZarAsync(int contractId, decimal amount, string currencyCode);
    }
}
/*
 Geekific. 2022. The Facade Pattern Explained and Implemented in Java | Structural 
Design Patterns | Geekific. [video online] Available at 
<https://youtu.be/xWk6jvqyhAQ?si=N8kiK-mawCrqQEa6> [Accessed 28 March 2026].
 */
