using GLMSMVC.Repositories.Interfaces;
using GLMSMVC.Services.Interfaces;
using GLMSMVC.Services.Results;

namespace GLMSMVC.Services.CurrencyConversion
{
    public class CurrencyConversionFacade : ICurrencyConversionFacade //(Geekific, 2022)
    {
        private readonly IContractRepository _contractRepository;
        private readonly IExchangeRateProvider _exchangeRateProvider;
        private readonly ICurrencyCalculator _currencyCalculator;

        public CurrencyConversionFacade(IContractRepository contractRepository, IExchangeRateProvider exchangeRateProvider, ICurrencyCalculator currencyCalculator)
        {
            _contractRepository = contractRepository;
            _exchangeRateProvider = exchangeRateProvider;
            _currencyCalculator = currencyCalculator;
        }

        public async Task<ServiceResult<decimal>> ConvertCostToZarAsync(int contractId, decimal amount, string currencyCode)
        {
            if (amount < 0)
            {
                return ServiceResult<decimal>.Failure("Amount cannot be negative.");
            }

            var contract = await _contractRepository.GetWithClientAsync(contractId);
            if (contract == null)
            {
                return ServiceResult<decimal>.Failure("Contract not found.");
            }

            if (string.IsNullOrWhiteSpace(currencyCode))
            {
                return ServiceResult<decimal>.Failure("Currency code is required.");
            }

            var rateResult = await _exchangeRateProvider.GetRateToZarAsync(currencyCode);
            if (!rateResult.IsSuccess || rateResult.Data == null)
            {
                return ServiceResult<decimal>.Failure(rateResult.Errors);
            }

            var rate = rateResult.Data;
            var convertedAmount = _currencyCalculator.ConvertToZar(amount, rate);

            return ServiceResult<decimal>.Success(convertedAmount);
        }
    }
}

/*
 Geekific. 2022. The Facade Pattern Explained and Implemented in Java | Structural 
Design Patterns | Geekific. [video online] Available at 
<https://youtu.be/xWk6jvqyhAQ?si=N8kiK-mawCrqQEa6> [Accessed 28 March 2026].
 */
