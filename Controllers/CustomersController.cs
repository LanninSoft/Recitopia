using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recitopia.Data;
using Recitopia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recitopia.Controllers
{
    public class CustomersController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public CustomersController(RecitopiaDBContext recitopiaDbContext)
        {
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
        }

        [Authorize(Roles = "Administrator")]
        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View(await _recitopiaDbContext.Customers.ToListAsync());
        }

        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            //BUILD VIEW FOR ANGULARJS RENDERING
            var customers = await _recitopiaDbContext.Customers.ToListAsync();

            return customers != null
                ? Json(customers)
                : Json(new { Status = "Failure" });
        }

        public async Task<IActionResult> CustomerLogin()
        {
            //RETURN LIST OF CUSTOMERS THAT A MEMBER OF
            //IF ONLY ONE, TAKE THEM HOME.  IF MULTIPLE, PROVIDE LIST AND USER SELECTS WHICH CUSTOMER TO LOGIN TO
            var currentUser = await _recitopiaDbContext.AppUsers
                .SingleAsync(m => m.UserName == User.Identity.Name);

            var customerIds = await _recitopiaDbContext.Customer_Users
                .Where(cu => cu.Id == currentUser.Id)
                .Select(cu => cu.Customer_Id)
                .ToListAsync();

            if (customerIds.Count() > 1)
            {
                //Provide selection view

                //create list and populate with Customer name and Id
                var custList = new List<List<string>>();

                foreach (var customerId in customerIds)
                {
                    var customer = await _recitopiaDbContext.Customers
                        .SingleAsync(m => m.Customer_Id == customerId);

                    // Dave: I'm not sure how this is used yet, but it seems like you need a keyvalue pair. A dictionary would 
                    // work better.
                    custList.Add(new List<string> { customer.Customer_Name, customer.Customer_Id.ToString() });
                }

                ViewBag.UserCustomers = custList;

                return View();
            }

            if (customerIds.Count() == 1)
            {
                //take them to home page
                var customerCId = await _recitopiaDbContext.Customer_Users.SingleAsync(m => m.Id == currentUser.Id);

                return RedirectToAction("CustomerLoginGo", new { id = customerCId.Customer_Id });
            }

            ViewBag.UserCustomers = null;

            return View();
        }

        public async Task<IActionResult> CustomerLoginGo(int id)
        {
            //save customerid to appuser field to carry


            var currentUser = await _recitopiaDbContext.AppUsers.Where(m => m.UserName ==  User.Identity.Name).FirstAsync();


            var getCustomerName = await _recitopiaDbContext.Customers.Where(m => m.Customer_Id == id).FirstAsync();

            currentUser.Customer_Id = id;
            currentUser.Customer_Name = getCustomerName.Customer_Name;

            HttpContext.Session.SetString("CurrentUserCustomerId", id.ToString());

            _recitopiaDbContext.SaveChanges();

            return LocalRedirect("~/Home/Index");
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _recitopiaDbContext.Customers
                .FirstOrDefaultAsync(m => m.Customer_Id == id);

            if (customers == null)
            {
                return NotFound();
            }

            return View(customers);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Customers customers)
        {
            if (ModelState.IsValid)
            {
                _recitopiaDbContext.Add(customers);
                await _recitopiaDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customers);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _recitopiaDbContext.Customers.FindAsync(id);

            if (customers == null)
            {
                return NotFound();
            }

            return View(customers);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Customers customers)
        {
            if (id != customers.Customer_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _recitopiaDbContext.Update(customers);
                    await _recitopiaDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CustomersExists(customers.Customer_Id))
                    {
                        return NotFound();
                    }

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customers);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _recitopiaDbContext.Customers
                .FirstOrDefaultAsync(m => m.Customer_Id == id);

            if (customers == null)
            {
                return NotFound();
            }

            return View(customers);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customers = await _recitopiaDbContext.Customers.FindAsync(id);
            _recitopiaDbContext.Customers.Remove(customers);
            await _recitopiaDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CustomersExists(int id)
        {
            return await _recitopiaDbContext.Customers.AnyAsync(e => e.Customer_Id == id);
        }
    }
}
