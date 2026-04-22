using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GLMSMVC.Data;
using GLMSMVC.Models.Domain;

namespace GLMSMVC.Controllers
{
    /// <summary>
    /// Parent Controller to ServiceRequest Child work item
    /// </summary>
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Contracts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Contracts.Include(c => c.Client);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id) // Show Contract and Client Info
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            return View();
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClientId,StartDate,EndDate,ServiceLevel,FilePath,Status")] Contract contract)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,StartDate,EndDate,ServiceLevel,FilePath,Status")] Contract contract)
        {
            if (id != contract.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

       

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
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