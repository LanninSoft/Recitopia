using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recitopia.Models;

namespace Recitopia.Controllers
{
    public class CustomersController : Controller
    {
        private readonly RecitopiaDBContext db;
        

        public CustomersController(RecitopiaDBContext context)
        {
            db = context;
        }
        [Authorize]
        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View(await db.Customers.ToListAsync());
        }
        public IActionResult CustomerLogin()
        {

            //RETURN LIST OF CUSTOMERS THAT A MEMBER OF
            //IF ONLY ONE, TAKE THEM HOME.  IF MULTIPLE, PROVIDE LIST AND USER SELECTS WHICH CUSTOMER TO LOGIN TO
            var currentUser = db.AppUsers.Where(m => m.UserName.Equals(User.Identity.Name)).First();

            var customerList = db.Customer_Users.Where(m => m.Id == currentUser.Id);

            if (customerList.Count() > 1)
            {
                //Provide selection view

                //create list and populate with Customer name and Id
                List<IList<string>> custList = new List<IList<string>>();

                foreach (Customer_Users thing in customerList)
                {
                    var tempResults = db.Customers.Where(m => m.Customer_Id == thing.Customer_Id).First();
                    
                    custList.Add(new List<string> { tempResults.Customer_Name, tempResults.Customer_Id.ToString() });
                }


                ViewBag.UserCustomers = custList;

                return View();
            }
            else if(customerList.Count() == 1)
            {
                //take them to home page
                var customerCId = db.Customer_Users.Where(m => m.Id == currentUser.Id).First();

                return RedirectToAction("CustomerLoginGo", new { id = customerCId.Customer_Id});
            }
            else
            {
                ViewBag.UserCustomers = null;

                return View();
            }

        }
        public IActionResult CustomerLoginGo (int id)
        {
            //save customerid to appuser field to carry
            var currentUser = db.AppUsers.Where(m => m.UserName.Equals(User.Identity.Name)).First();
                                  

            var getCustomerName = db.Customers.Where(m => m.Customer_Id == id).First();

            currentUser.Customer_Id = id;
            currentUser.Customer_Name = getCustomerName.Customer_Name;

            HttpContext.Session.SetString("CurrentUserCustomerId", id.ToString());

            db.SaveChanges();

            return LocalRedirect("~/Home/Index");
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await db.Customers
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
                db.Add(customers);
                await db.SaveChangesAsync();
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

            var customers = await db.Customers.FindAsync(id);
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
                    db.Update(customers);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomersExists(customers.Customer_Id))
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
            return View(customers);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await db.Customers
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
            var customers = await db.Customers.FindAsync(id);
            db.Customers.Remove(customers);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomersExists(int id)
        {
            return db.Customers.Any(e => e.Customer_Id == id);
        }
    }
}
