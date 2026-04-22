using GLMSMVC.Models.Domain;
using GLMSMVC.Models.DTOs;
using GLMSMVC.Models.Enums;
using GLMSMVC.Repositories.Interfaces;
using GLMSMVC.Services.Interfaces;
using GLMSMVC.Services.Results;
using GLMSMVC.Services.States;
using Microsoft.EntityFrameworkCore;

namespace GLMSMVC.Services.Core_Services
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IContractFactory _contractFactory;
        private readonly IServiceRequestFactory _serviceRequestFactory;
        private readonly IContractFileService _contractFileService;
        private readonly ContractStateResolver _contractStateResolver;

        public ContractService(IContractRepository contractRepository, IClientRepository clientRepository, IServiceRequestRepository serviceRequestRepository, 
            IContractFactory contractFactory, IServiceRequestFactory serviceRequestFactory, IContractFileService contractFileService, ContractStateResolver contractStateResolver)
        {
            _contractRepository = contractRepository;
            _clientRepository = clientRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _contractFactory = contractFactory;
            _serviceRequestFactory = serviceRequestFactory;
            _contractFileService = contractFileService;
            _contractStateResolver = contractStateResolver;
        }

        public async Task<ServiceResult<IEnumerable<Contract>>> GetAllAsync()
        {
            var contracts = await _contractRepository.GetAllAsync();
            return ServiceResult<IEnumerable<Contract>>.Success(contracts);
        }

        public async Task<ServiceResult<Contract>> GetByIdAsync(int id)
        {
            var contract = await _contractRepository.GetWithServiceRequestAsync(id);

            if (contract == null)
            {
                return ServiceResult<Contract>.Failure("Contract not found.");
            }

            return ServiceResult<Contract>.Success(contract);
        }

        // For Search/Filter in list view
        public async Task<ServiceResult<IEnumerable<Contract>>> GetFilteredAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            if (startDate.HasValue && endDate.HasValue && startDate.Value.Date > endDate.Value.Date)
            {
                return ServiceResult<IEnumerable<Contract>>.Failure("Start date cannot be later than end date.");
            }

            var contracts = await _contractRepository.GetFilteredAsync(startDate, endDate, status);
            return ServiceResult<IEnumerable<Contract>>.Success(contracts);
        }

        public async Task<ServiceResult<Contract>> CreateAsync(CreateContractDTO dto, IFormFile? file)
        {
            // Client Exists, Valid Date
            var errors = await ValidateContractDtoAsync(dto);
            if (errors.Any())
            {
                return ServiceResult<Contract>.Failure(errors);
            }

            // Create contract first with no file path yet
            var contract = _contractFactory.Create(dto, null);

            // Evaluate first state 
            contract.Status = _contractStateResolver.Evaluate(contract);

            await _contractRepository.AddAsync(contract);
            await _contractRepository.SaveChangesAsync();

            // Upload file after contract exists so contractId can be used in filename
            if (file != null)
            {
                var uploadResult = await _contractFileService.UploadFileAsync(contract.Id, file);
                if (!uploadResult.IsSuccess)
                {
                    return ServiceResult<Contract>.Failure(uploadResult.Errors);
                }

                contract.FilePath = uploadResult.Data;
            }
            // Evaluate State after file upload
            contract.Status = _contractStateResolver.Evaluate(contract);

            _contractRepository.Update(contract);
            await _contractRepository.SaveChangesAsync();

            // Check if status Active to create Service Request 
            await EnsurePendingServiceRequestExistsIfActiveAsync(contract);

            return ServiceResult<Contract>.Success(contract);
        }

        public async Task<ServiceResult<Contract>> UpdateAsync(int id, CreateContractDTO dto, IFormFile? file, bool pauseContract)
        {
            var contract = await _contractRepository.GetWithServiceRequestAsync(id);
            if (contract == null)
            {
                return ServiceResult<Contract>.Failure("Contract not found.");
            }

            if (!_contractStateResolver.CanEdit(contract))
            {
                return ServiceResult<Contract>.Failure("Only Draft or On Hold contracts can be edited.");
            }

            // Client Exists, Valid Date
            var errors = await ValidateContractDtoAsync(dto);
            if (errors.Any())
            {
                return ServiceResult<Contract>.Failure(errors);
            }

            try
            {
                contract.ClientId = dto.ClientId;
                contract.StartDate = dto.StartDate;
                contract.EndDate = dto.EndDate;
                contract.ServiceLevel = dto.ServiceLevel;
                contract.IsPaused = pauseContract;

                if (file != null)
                {
                    var uploadResult = await _contractFileService.UploadFileAsync(contract.Id, file);
                    if (!uploadResult.IsSuccess)
                    {
                        return ServiceResult<Contract>.Failure(uploadResult.Errors);
                    }

                    contract.FilePath = uploadResult.Data;
                }

                contract.Status = _contractStateResolver.Evaluate(contract);

                _contractRepository.Update(contract);
                await _contractRepository.SaveChangesAsync();

                // Handles Creation/Update of related Service Request after contract is updated
                await HandleServiceRequestAfterContractUpdateAsync(contract);

                return ServiceResult<Contract>.Success(contract);
            }
            catch (DbUpdateConcurrencyException)
            {
                return ServiceResult<Contract>.Failure("This contract was modified by another user. Please reload and try again.");
            }
        }
        // Client Exists, Valid Date
        private async Task<List<string>> ValidateContractDtoAsync(CreateContractDTO dto)
        {
            var errors = new List<string>();

            if (!await _clientRepository.ExistsAsync(dto.ClientId))
                errors.Add("Selected client does not exist.");

            if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.EndDate.Value.Date < dto.StartDate.Value.Date)
                errors.Add("End date cannot be before start date.");

            return errors;
        }

        // Check if status Active to create Service Request 
        private async Task EnsurePendingServiceRequestExistsIfActiveAsync(Contract contract)
        {
            if (contract.Status != ContractStatus.Active)
                return;

            var existingSr = await _serviceRequestRepository.GetByContractIdAsync(contract.Id);
            if (existingSr != null)
                return;

            var newSr = _serviceRequestFactory.CreateFromContract(
                contract,
                description: "Auto-created pending service request.",
                originalAmount: 0,
                sourceCurrency: "USD",
                costZar: 0,
                usedApi: false);

            await _serviceRequestRepository.AddAsync(newSr);
            await _serviceRequestRepository.SaveChangesAsync();
        }

        // Handles Creation/Update of related Service Request after contract is updated
        private async Task HandleServiceRequestAfterContractUpdateAsync(Contract contract)
        {
            var existingSr = await _serviceRequestRepository.GetByContractIdAsync(contract.Id);

            if (contract.Status == ContractStatus.Active)
            {
                // Create and populate Service Request Defaults
                if (existingSr == null)
                {
                    var newSr = _serviceRequestFactory.CreateFromContract(
                        contract,
                        description: "Auto-created pending service request.",
                        originalAmount: 0,
                        sourceCurrency: "USD",
                        costZar: 0,
                        usedApi: false);

                    await _serviceRequestRepository.AddAsync(newSr);
                    await _serviceRequestRepository.SaveChangesAsync();
                }
            }

            // If Contract is on hold then set Service Request as on Hold
            else if (contract.Status == ContractStatus.OnHold)
            {
                if (existingSr != null)
                {
                    existingSr.Status = ServiceRequestStatus.OnHold;
                    _serviceRequestRepository.Update(existingSr);
                    await _serviceRequestRepository.SaveChangesAsync();
                }
            }
        }
    }
}
