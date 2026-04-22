using GLMSMVC.Data;
using GLMSMVC.Models.Domain;
using GLMSMVC.Models.Enums;
using GLMSMVC.Models.ViewModels;
using GLMSMVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;

namespace GLMSMVC.Controllers
{
    /// <summary>
    /// Child work item to ContractsController.
    /// </summary>
    public class ServiceRequestsController : Controller
    {
        private readonly IServiceRequestService _serviceRequestService;
        private readonly IWebHostEnvironment _environment;
        private readonly IContractFileService _contractFileService;

        public ServiceRequestsController(
            IServiceRequestService serviceRequestService,
            IWebHostEnvironment environment,
            IContractFileService contractFileService)
        {
            _serviceRequestService = serviceRequestService;
            _environment = environment;
            _contractFileService = contractFileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _serviceRequestService.GetAllAsync();

            var vm = new ServiceRequestIndexViewModel();

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return View(vm);
            }

            vm.Items = (result.Data ?? Enumerable.Empty<GLMSMVC.Models.Domain.ServiceRequest>())
                .Select(sr => new ServiceRequestIndexItemViewModel
                {
                    Id = sr.Id,
                    ContractId = sr.ContractId,
                    ClientName = sr.Contract?.Client?.Name ?? "",
                    ContractStatus = sr.Contract?.Status.ToString() ?? "",
                    Description = sr.Description,
                    CostZar = sr.CostZar,
                    ServiceRequestStatus = sr.Status.ToString()
                })
                .ToList();

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _serviceRequestService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }

            var sr = result.Data;

            var vm = new ServiceRequestDetailsViewModel
            {
                Id = sr.Id,
                ClientName = sr.Contract?.Client?.Name ?? "",
                ContractId = sr.ContractId,
                StartDate = sr.Contract?.StartDate,
                EndDate = sr.Contract?.EndDate,
                ServiceLevel = sr.Contract?.ServiceLevel?.ToString(),
                ServiceRequestStatus = sr.Status.ToString(),
                CanDownloadFile = !string.IsNullOrWhiteSpace(sr.Contract?.FilePath)
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _serviceRequestService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }

            var sr = result.Data;
            var clientRegion = sr.Contract?.Client?.Region ?? "";
            var selectedCurrency = await ResolveCurrencyCodeFromCountryAsync(clientRegion);
            if (string.IsNullOrWhiteSpace(selectedCurrency))
            {
                selectedCurrency = sr.SourceCurrency;
            }

            var vm = new ServiceRequestEditViewModel
            {
                Id = sr.Id,
                ContractId = sr.ContractId,
                ClientName = sr.Contract?.Client?.Name ?? "",
                ContractStatus = sr.Contract?.Status.ToString() ?? "",
                Description = sr.Description,
                OriginalAmount = sr.OriginalAmount,
                CurrencyCode = selectedCurrency,
                CostZar = sr.CostZar,
                Status = sr.Status,
                UseApi = true,
                IsReadOnly = sr.Status == ServiceRequestStatus.Completed || sr.Status == ServiceRequestStatus.Cancelled
            };

            await PopulateServiceRequestOptionsAsync(vm, clientRegion);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequestEditViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            await PopulateServiceRequestOptionsAsync(vm, null);

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _serviceRequestService.UpdateAsync(
                vm.Id,
                vm.Description,
                vm.OriginalAmount,
                vm.CurrencyCode,
                vm.Status,
                vm.UseApi);

            if (!result.IsSuccess)
            {
                AddErrorsToModelState(result.Errors);

                var joined = string.Join(" ", result.Errors);
                if (joined.Contains("api", StringComparison.OrdinalIgnoreCase) ||
                    joined.Contains("unavailable", StringComparison.OrdinalIgnoreCase) ||
                    joined.Contains("timed out", StringComparison.OrdinalIgnoreCase))
                {
                    vm.ApiError = result.ErrorMessage ?? "Currency API is currently unavailable.";
                }

                return View(vm);
            }

            TempData["SuccessMessage"] = "Service request updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DownloadContractFile(int id)
        {
            var result = await _serviceRequestService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }

            var contractId = result.Data.ContractId;
            var fileResult = await _contractFileService.DownloadFileAsync(contractId);

            if (!fileResult.IsSuccess || fileResult.Data == null)
            {
                TempData["ErrorMessage"] = fileResult.ErrorMessage;
                return RedirectToAction(nameof(Details), new { id });
            }

            return File(fileResult.Data.FileBytes, fileResult.Data.ContentType, fileResult.Data.FileName);
        }

        private async Task PopulateServiceRequestOptionsAsync(ServiceRequestEditViewModel vm, string? clientRegion)
        {
            vm.StatusOptions = Enum.GetValues<ServiceRequestStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString(),
                    Selected = vm.Status == s
                })
                .ToList();

            var countries = await LoadCountriesAsync();
            var selectedCode = vm.CurrencyCode;

            if (string.IsNullOrWhiteSpace(selectedCode) && !string.IsNullOrWhiteSpace(clientRegion))
            {
                selectedCode = countries
                    .FirstOrDefault(c => c.Country.Equals(clientRegion, StringComparison.OrdinalIgnoreCase))
                    ?.CurrencyCode ?? "USD";
            }

            vm.CurrencyOptions = countries
                .OrderBy(c => c.Country)
                .Select(c => new SelectListItem
                {
                    Value = c.CurrencyCode,
                    Text = $"{c.Country} ({c.CurrencyCode})",
                    Selected = c.CurrencyCode.Equals(selectedCode, StringComparison.OrdinalIgnoreCase)
                })
                .ToList();
        }

        private async Task<string?> ResolveCurrencyCodeFromCountryAsync(string countryName)
        {
            var countries = await LoadCountriesAsync();

            return countries
                .FirstOrDefault(c => c.Country.Equals(countryName, StringComparison.OrdinalIgnoreCase))
                ?.CurrencyCode;
        }

        private async Task<List<CountryOption>> LoadCountriesAsync()
        {
            var path = Path.Combine(_environment.ContentRootPath, "CountryInfo.json");
            if (!System.IO.File.Exists(path))
            {
                return new List<CountryOption>();
            }

            await using var stream = System.IO.File.OpenRead(path);
            var countries = await JsonSerializer.DeserializeAsync<List<CountryOption>>(stream);
            return countries ?? new List<CountryOption>();
        }

        private void AddErrorsToModelState(IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        private class CountryOption
        {
            public string CurrencyCode { get; set; } = string.Empty;
            public string CurrencyName { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
        }
    }
}
/*
SERVICE REQUEST VIEWs

Index - List Id,ContractId, Client Name, Contract Status, Description,Cost ,SR Status, Actions (Edit, Details)
Edit - Description, Automatically get Client Region Code and populate as the selected option for a dropdown that shows a list of Countries by Country name that hold the represented country code for conversion to ZAR (south african Rand) when Save Changes; If the user wants to overide the entered currency amount to another they can select it from the dropdown list; it parses the relevant country code to facade to do conversion and save the Rand value to database, SR Status as Status(Completed, Cancelled, On Hold, Pending) Dropdown
Details - Client Name, Contract Id, Start Date,End Date,ServiceLevel, Download for File, SR Status

Service Request Status (Selected by user in dropdown)
Pending: Work is awaiting processing/In progress; Contract stays Active
Completed: Work finished successfully and becomes read-only after this; Contract stays Active
Cancelled: Stopped permanently and becomes read-only after this; Contract usually stays Active
On Hold: Service work paused temporarily; Contract may stay active, unless contract is paused (Contract Status Changes to On hold)
    Parent to Child change allowed for On Hold
    Contract OnHold sets Service Request to On Hold

Using Facade for currency conversion Process:
Connect to API with async
Handle error if API service is down, 



*/

/*
 Design Pattern Changes from planning (part 1)

Factroy Pattern: Creation of Contract object only.
State Pattern: Contract Lifecycle for each behavoiur of Contract Status
    Rework classes per my updated Logic. Not per Part 1 plan.
Facade Pattern: Provide a single service for currency conversion.
    Keep simmilar Facade logic as part 1

Have Services to define data access operations
IClientRepository
IContractRepository
IServiceRequestRepository
 
 */