using GLMSMVC.Models.Domain;
using GLMSMVC.Models.Enums;
using GLMSMVC.Repositories.Interfaces;
using GLMSMVC.Services.Interfaces;
using GLMSMVC.Services.Results;
using GLMSMVC.Services.States;

namespace GLMSMVC.Services.Core_Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IServiceRequestFactory _serviceRequestFactory;
        private readonly ICurrencyConversionFacade _currencyConversionFacade;
        private readonly ContractStateResolver _contractStateResolver;

        public ServiceRequestService(IServiceRequestRepository serviceRequestRepository, IContractRepository contractRepository, IServiceRequestFactory serviceRequestFactory,
            ICurrencyConversionFacade currencyConversionFacade, ContractStateResolver contractStateResolver)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _contractRepository = contractRepository;
            _serviceRequestFactory = serviceRequestFactory;
            _currencyConversionFacade = currencyConversionFacade;
            _contractStateResolver = contractStateResolver;
        }

        // Retrieves all service requests
        public async Task<ServiceResult<IEnumerable<ServiceRequest>>> GetAllAsync()
        {
            var serviceRequests = await _serviceRequestRepository.GetAllAsync();
            return ServiceResult<IEnumerable<ServiceRequest>>.Success(serviceRequests);
        }

        // Retrieves a single service request by id including its parent contract.
        public async Task<ServiceResult<ServiceRequest>> GetByIdAsync(int id)
        {
            var serviceRequest = await _serviceRequestRepository.GetWithContractAsync(id);

            if (serviceRequest == null)
            {
                return ServiceResult<ServiceRequest>.Failure("Service request not found.");
            }

            return ServiceResult<ServiceRequest>.Success(serviceRequest);
        }

        // Create Service Request for given contract
        // Perform input validation: Ensuring valid contract state as Active
        // Pervents duplicates, converts provided amount to Rand via Facade
        public async Task<ServiceResult<ServiceRequest>> CreateAsync(int contractId, string description, decimal originalAmount, string currencyCode)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(description))
                errors.Add("Description is required.");

            if (originalAmount < 0)
                errors.Add("Amount cannot be negative.");

            if (string.IsNullOrWhiteSpace(currencyCode))
                errors.Add("Currency code is required.");

            // Return All Errors
            if (errors.Any())
                return ServiceResult<ServiceRequest>.Failure(errors);

            // Load parent contract including any existing service request
            var contract = await _contractRepository.GetWithServiceRequestAsync(contractId);
            if (contract == null)
            {
                return ServiceResult<ServiceRequest>.Failure("Parent contract not found.");
            }

            // Re-evaluate current status before using it
            contract.Status = _contractStateResolver.Evaluate(contract);

            // Ensure contract allows creation of a service request
            if (!_contractStateResolver.CanCreateServiceRequest(contract))
            {
                return ServiceResult<ServiceRequest>.Failure("A service request cannot be created for Expired, Draft, or On Hold contracts.");
            }

            // Prevent duplicate service requests for the same contract
            var existingSr = await _serviceRequestRepository.GetByContractIdAsync(contractId);
            if (existingSr != null)
            {
                return ServiceResult<ServiceRequest>.Failure("A service request already exists for this contract.");
            }

            // Convert amt to ZAR using facade
            var conversionResult = await _currencyConversionFacade.ConvertCostToZarAsync(contractId, originalAmount, currencyCode);
            if (!conversionResult.IsSuccess || conversionResult.Data == null)
            {
                return ServiceResult<ServiceRequest>.Failure(conversionResult.Errors);
            }

            // Create & Save Service request 
            var serviceRequest = _serviceRequestFactory.CreateFromContract(
                contract,
                description,
                originalAmount,
                currencyCode,
                conversionResult.Data,
                usedApi: true);

            await _serviceRequestRepository.AddAsync(serviceRequest);
            await _serviceRequestRepository.SaveChangesAsync();

            return ServiceResult<ServiceRequest>.Success(serviceRequest);
        }

        // Update exsisting Service Request's Properties
        // Validate inputs, prevent editing for compleated/cancelled requests
        // Can use coutry code to convert to different to Client.Region currency
        public async Task<ServiceResult<ServiceRequest>> UpdateAsync(int id, string description, decimal originalAmount,
            string currencyCode, ServiceRequestStatus status, bool useApi)
        {
            // Get Service Request and Contract
            var serviceRequest = await _serviceRequestRepository.GetWithContractAsync(id);
            if (serviceRequest == null)
            {
                return ServiceResult<ServiceRequest>.Failure("Service request not found.");
            }

            // Completed or cancelled requests arn't ediable
            if (serviceRequest.Status == ServiceRequestStatus.Completed ||
                serviceRequest.Status == ServiceRequestStatus.Cancelled)
            {
                return ServiceResult<ServiceRequest>.Failure("Completed or Cancelled service requests are read-only.");
            }

            var errors = new List<string>();

            // Input Validation
            if (string.IsNullOrWhiteSpace(description))
                errors.Add("Description is required.");

            if (originalAmount < 0)
                errors.Add("Amount cannot be negative.");

            if (string.IsNullOrWhiteSpace(currencyCode))
                errors.Add("Currency code is required.");

            if (errors.Any())
                return ServiceResult<ServiceRequest>.Failure(errors);

            var contract = serviceRequest.Contract;
            if (contract == null)
            {
                return ServiceResult<ServiceRequest>.Failure("Parent contract could not be loaded.");
            }

            // Re-evaluate Status
            contract.Status = _contractStateResolver.Evaluate(contract);

            // Parent forces child OnHold
            if (contract.Status == ContractStatus.OnHold)
            {
                status = ServiceRequestStatus.OnHold;
            }

            // Apply Updates to Service Request
            serviceRequest.Description = description;
            serviceRequest.OriginalAmount = originalAmount;
            serviceRequest.SourceCurrency = currencyCode.ToUpperInvariant();
            serviceRequest.Status = status;

            // Get latest rates
            if (useApi)
            {
                var conversionResult = await _currencyConversionFacade.ConvertCostToZarAsync(contract.Id, originalAmount, currencyCode);
                if (!conversionResult.IsSuccess || conversionResult.Data == null)
                {
                    return ServiceResult<ServiceRequest>.Failure(conversionResult.Errors);
                }

                serviceRequest.CostZar = conversionResult.Data;
                serviceRequest.UsedAPI = true;
            }
            else
            {
                // When not using the API, mark the record as not having used the conversion API
                serviceRequest.UsedAPI = false;
            }

            // Apply changes
            _serviceRequestRepository.Update(serviceRequest);
            await _serviceRequestRepository.SaveChangesAsync();

            return ServiceResult<ServiceRequest>.Success(serviceRequest);
        }
    }
}
