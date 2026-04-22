using GLMSMVC.Models.DTOs;
using GLMSMVC.Models.Enums;
using GLMSMVC.Models.ViewModels;
using GLMSMVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMSMVC.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IContractService _contractService;
        private readonly IClientService _clientService;
        private readonly IContractFileService _contractFileService;

        public ContractsController(
            IContractService contractService,
            IClientService clientService,
            IContractFileService contractFileService)
        {
            _contractService = contractService;
            _clientService = clientService;
            _contractFileService = contractFileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            var result = await _contractService.GetFilteredAsync(startDate, endDate, status);

            var vm = new ContractIndexViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Status = status,
                StatusOptions = BuildContractStatusOptions(status)
            };

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return View(vm);
            }

            vm.Items = (result.Data ?? Enumerable.Empty<GLMSMVC.Models.Domain.Contract>())
                .Select(c => new ContractIndexItemViewModel
                {
                    Id = c.Id,
                    ClientName = c.Client?.Name ?? "",
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    ServiceLevel = c.ServiceLevel?.ToString(),
                    ContractStatus = c.Status.ToString(),
                    CanEdit = c.Status == ContractStatus.Draft || c.Status == ContractStatus.OnHold
                })
                .ToList();

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _contractService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }

            var contract = result.Data;

            var vm = new ContractDetailsViewModel
            {
                Id = contract.Id,
                ClientName = contract.Client?.Name ?? "",
                Region = contract.Client?.Region ?? "",
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                ServiceLevel = contract.ServiceLevel?.ToString(),
                ContractStatus = contract.Status.ToString(),
                HasFile = !string.IsNullOrWhiteSpace(contract.FilePath)
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new ContractFormViewModel();
            await PopulateContractFormOptionsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContractFormViewModel vm)
        {
            await PopulateContractFormOptionsAsync(vm);

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var dto = new CreateContractDTO
            {
                ClientId = vm.ClientId,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate,
                ServiceLevel = vm.ServiceLevel
            };

            var result = await _contractService.CreateAsync(dto, vm.File);

            if (!result.IsSuccess)
            {
                AddErrorsToModelState(result.Errors);
                return View(vm);
            }

            TempData["SuccessMessage"] = "Contract created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _contractService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }

            var contract = result.Data;

            if (contract.Status != ContractStatus.Draft && contract.Status != ContractStatus.OnHold)
            {
                TempData["ErrorMessage"] = "Only Draft or On Hold contracts can be edited.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new ContractFormViewModel
            {
                Id = contract.Id,
                ClientId = contract.ClientId,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                ServiceLevel = contract.ServiceLevel,
                PauseContract = contract.IsPaused,
                ExistingFilePath = contract.FilePath
            };

            await PopulateContractFormOptionsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContractFormViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            await PopulateContractFormOptionsAsync(vm);

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var dto = new CreateContractDTO
            {
                ClientId = vm.ClientId,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate,
                ServiceLevel = vm.ServiceLevel
            };

            var result = await _contractService.UpdateAsync(id, dto, vm.File, vm.PauseContract);

            if (!result.IsSuccess)
            {
                AddErrorsToModelState(result.Errors);
                return View(vm);
            }

            TempData["SuccessMessage"] = "Contract updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var result = await _contractFileService.DownloadFileAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction(nameof(Details), new { id });
            }

            return File(result.Data.FileBytes, result.Data.ContentType, result.Data.FileName);
        }

        private async Task PopulateContractFormOptionsAsync(ContractFormViewModel vm)
        {
            var clientResult = await _clientService.GetAllAsync();

            vm.ClientOptions = (clientResult.Data ?? Enumerable.Empty<GLMSMVC.Models.Domain.Client>())
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == vm.ClientId
                })
                .ToList();

            vm.ServiceLevelOptions = Enum.GetValues<ServiceLevel>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString(),
                    Selected = vm.ServiceLevel == s
                })
                .ToList();
        }

        private List<SelectListItem> BuildContractStatusOptions(ContractStatus? selected)
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "All Statuses", Selected = selected == null }
            };

            list.AddRange(Enum.GetValues<ContractStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString(),
                    Selected = selected == s
                }));

            return list;
        }

        private void AddErrorsToModelState(IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }
    }
}
/*
CONTRACTS VIEWs

Index - list Client Name, Start End Dates, Service Level, Contract Status, Actions(Edit, Details)
    Show Action Edit If Contract Status Draft OR On Hold
    Hide Action Edit When Contract Staus Expired OR Active

Index Filter By Date Range Start - End ... valid range AND Status with Filter/Search Btn above list


Details - Client Name, Region as Country Name & Contract Start End Dates, Service Level, File Download, Contract Status
Edit - ClientId as Client Name dropdown,StartDate,EndDate,ServiceLevel as DropDown (Standard, Express, Gold), Pause Contract: Checkbox ticked to put Contract Status On Hold; untick check if incomplete for Draft OR of expired by date for Expired OR set as Active,FilePath as File Upload PDF only with exception handling
Create - ClientId as Client Name dropdown, StartDate,EndDate, ServiceLevel as DropDown (Standard, Express, Gold),FilePath as File Upload PDF only with exception handling
 



Contract Status (pause checkbox for On Hold, state evaluation for Draft / Active / Expired)
Draft: Contract Incomplete missing either Start End Date OR Service Level, Uploaded File ; Cannot Create Service Request
Active: Contract valid and visable, Cannot Edit Contract anymore; Service Request gets automatically created if no Service Request exists create one with Service Request Status as Pending; if a SR exists, dont create another
Expired: Contract no longer valid by End Date; New Service Request cannot be created
On Hold: Contract paused by Admin; Existing Service Request becomes On Hold


Files uploaded are saved to wwwroot/uploads
View validation only allows PDF files
Backend Logic also validates for only allowing PDF files  (as part of service)
Files can be downloaded.
Files get named with Guid.NewGuid()_contract_{contractId}.pdf
If a file is uploaded and that contract already has a file uploaded it overwrites the old file with the new uploaded file
Validation in Model/Controller entry AND service Layer

Handle for concurency Conflicts for Contract State





 */