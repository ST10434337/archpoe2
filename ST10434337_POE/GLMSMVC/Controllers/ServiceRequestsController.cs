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
    /// Child work item to ContractsController.
    /// </summary>
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ServiceRequests.Include(s => s.Contract);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        

        // GET: ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null)
            {
                return NotFound();
            }
            ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "Id", serviceRequest.ContractId);
            return View(serviceRequest);
        }

        // POST: ServiceRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ContractId,Description,CostZar,Status")] ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRequestExists(serviceRequest.Id))
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
            ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "Id", serviceRequest.ContractId);
            return View(serviceRequest);
        }

        

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.Id == id);
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