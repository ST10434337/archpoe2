using GLMSMVC.Models.Domain;
using GLMSMVC.Models.ViewModels;
using GLMSMVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace GLMSMVC.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IWebHostEnvironment _environment;

        public ClientsController(IClientService clientService, IWebHostEnvironment environment)
        {
            _clientService = clientService;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _clientService.GetAllAsync();
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return View(new List<Client>());
            }

            return View(result.Data ?? Enumerable.Empty<Client>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _clientService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }

            var vm = new ClientDetailsViewModel
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                ContactDetails = result.Data.ContactDetails,
                Region = result.Data.Region
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new ClientFormViewModel();
            vm.RegionOptions = await BuildCountryRegionOptionsAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientFormViewModel vm)
        {
            vm.RegionOptions = await BuildCountryRegionOptionsAsync();

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var client = new Client
            {
                Name = vm.Name,
                ContactDetails = vm.ContactDetails,
                Region = vm.Region
            };

            var result = await _clientService.CreateAsync(client);
            if (!result.IsSuccess)
            {
                AddErrorsToModelState(result.Errors);
                return View(vm);
            }

            TempData["SuccessMessage"] = "Client created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _clientService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }

            var vm = new ClientFormViewModel
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                ContactDetails = result.Data.ContactDetails,
                Region = result.Data.Region,
                RegionOptions = await BuildCountryRegionOptionsAsync(result.Data.Region)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClientFormViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            vm.RegionOptions = await BuildCountryRegionOptionsAsync(vm.Region);

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var client = new Client
            {
                Id = vm.Id,
                Name = vm.Name,
                ContactDetails = vm.ContactDetails,
                Region = vm.Region
            };

            var result = await _clientService.UpdateAsync(client);
            if (!result.IsSuccess)
            {
                AddErrorsToModelState(result.Errors);
                return View(vm);
            }

            TempData["SuccessMessage"] = "Client updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private void AddErrorsToModelState(IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        private async Task<List<SelectListItem>> BuildCountryRegionOptionsAsync(string? selected = null)
        {
            var countries = await LoadCountriesAsync();

            return countries
                .OrderBy(c => c.Country)
                .Select(c => new SelectListItem
                {
                    Value = c.Country,
                    Text = c.Country,
                    Selected = c.Country == selected
                })
                .ToList();
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

        private class CountryOption
        {
            public string CurrencyCode { get; set; } = string.Empty;
            public string CurrencyName { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
        }
    }
}
/*
CLIENT VIEWs

Index - List Id,Name,Contact Details,Region as Country Name, Actions (Edit, Details)
    Has Create BTn on top right
Details - Client Name, Contact Details, Region as Country Name, Total Contracts count, Total Compleated Service Requests
Create - Name,Contact Details,Region as Country Name dropdown
Edit - Name,Contact Details,Region as Country Name dropdown

 */