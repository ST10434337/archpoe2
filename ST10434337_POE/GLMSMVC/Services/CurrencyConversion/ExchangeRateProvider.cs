using GLMSMVC.Models.DTOs;
using GLMSMVC.Services.Interfaces;
using GLMSMVC.Services.Results;
using System.Text.Json;

namespace GLMSMVC.Services
{
    public class ExchangeRateProvider : IExchangeRateProvider//(Geekific, 2022)
    {
        private readonly HttpClient _httpClient;

        public ExchangeRateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResult<decimal>> GetRateToZarAsync(string sourceCurrencyCode) //(ExchangeRate-API, n.d.)
        {
            if (string.IsNullOrWhiteSpace(sourceCurrencyCode))
            {
                return ServiceResult<decimal>.Failure("Source currency code is required.");
            }

            sourceCurrencyCode = sourceCurrencyCode.Trim().ToUpperInvariant();

            try
            {
                // Connect to API
                var response = await _httpClient.GetAsync($"https://open.er-api.com/v6/latest/{sourceCurrencyCode}");//(ExchangeRate-API, n.d.)

                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<decimal>.Failure("Currency API request failed.");
                }

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var apiResult = JsonSerializer.Deserialize<ExchangeRateResponseDTO>(json, options);

                if (apiResult == null)
                {
                    return ServiceResult<decimal>.Failure("Currency API returned no data.");
                }

                if (!string.Equals(apiResult.Result, "success", StringComparison.OrdinalIgnoreCase))
                {
                    return ServiceResult<decimal>.Failure("Currency API did not return a successful result.");
                }

                if(apiResult.Rates == null || !apiResult.Rates.ContainsKey("ZAR")) 
                {
                    return ServiceResult<decimal>.Failure("ZAR exchange rate was not found.");
                }

                // Finally if success send back ZAR rate
                decimal zarRate = apiResult.Rates["ZAR"];
                return ServiceResult<decimal>.Success(zarRate);
            }
            // Could implement SR prop like UsedAPI if false then display feedback to user that the converted amount is not accurate or something
            catch (HttpRequestException)
            {
                return ServiceResult<decimal>.Failure("Currency API is unavailable right now.");
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<decimal>.Failure("Currency API request timed out.");
            }
            catch (Exception)
            {
                return ServiceResult<decimal>.Failure("Unexpected error occurred during currency conversion.");
            }
        }
    }
}

/*
 Geekific. 2022. The Facade Pattern Explained and Implemented in Java | Structural 
Design Patterns | Geekific. [video online] Available at 
<https://youtu.be/xWk6jvqyhAQ?si=N8kiK-mawCrqQEa6> [Accessed 28 March 2026].
ExchangeRate-API. n.d. Open Access, No Key Required. [online] Available at: https://www.exchangerate-api.com/docs/free [Accessed 22 April 2026].
 */
