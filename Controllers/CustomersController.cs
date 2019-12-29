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

        public async Task<IActionResult> CustomerLogin(bool? clickLinktoChange)
        {
            var customerCount = 0;
            //RETURN LIST OF CUSTOMERS THAT A MEMBER OF
            //IF ONLY ONE, TAKE THEM HOME.  IF MULTIPLE, PROVIDE LIST AND USER SELECTS WHICH CUSTOMER TO LOGIN TO
            if (User.Identity.Name != null)
            {

                var currentUser = await _recitopiaDbContext.AppUsers
                .SingleAsync(m => m.UserName == User.Identity.Name);

                var customerIds = await _recitopiaDbContext.Customer_Users
                    .Where(cu => cu.User_Id == currentUser.Id)
                    .Select(cu => cu.Customer_Guid)
                    .ToListAsync();
            
                customerCount = customerIds.Count();

                if (customerCount > 1 && clickLinktoChange != true)
                {

                    //Look to see if company id and name saved prior.  If so, bypass selection page and take to home
                    var checkLastLoginCompanyInfo = await _recitopiaDbContext.AppUsers.SingleAsync(m => m.Id == currentUser.Id);

                    if (checkLastLoginCompanyInfo.Customer_Guid != null && checkLastLoginCompanyInfo.Customer_Name != null)
                    {
                         var customerGuid = await _recitopiaDbContext.Customers.SingleAsync(m => m.Customer_Guid == checkLastLoginCompanyInfo.Customer_Guid);

                        HttpContext.Session.SetString("CurrentUserCustomerGuid", customerGuid.Customer_Guid);

                        return LocalRedirect("~/Home/Index");
                    }
                    else
                    {
                        //Provide selection view
                        //create list and populate with Customer name and Id
                        List<IList<string>> custList = new List<IList<string>>();

                        foreach (var customerId in customerIds)
                        {
                            var tempResults = await _recitopiaDbContext.Customers.SingleAsync(m => m.Customer_Guid == customerId);

                            custList.Add(new List<string> { tempResults.Customer_Name, tempResults.Customer_Guid });
                        }
                        ViewBag.UserCustomers = custList;

                        return View();
                    }
                }
                else if (customerCount > 1 && clickLinktoChange == true)
                {
                    //Provide selection view
                    //create list and populate with Customer name and Id
                    List<IList<string>> custList = new List<IList<string>>();

                    foreach (var customerId in customerIds)
                    {
                        var tempResults = await _recitopiaDbContext.Customers.SingleAsync(m => m.Customer_Guid == customerId);

                        custList.Add(new List<string> { tempResults.Customer_Name, tempResults.Customer_Guid });
                    }
                    ViewBag.UserCustomers = custList;

                    return View();
                }
                else if (customerCount == 1)
                {
                    //take them to home page
                    var customerCId = await _recitopiaDbContext.Customer_Users.SingleAsync(m => m.User_Id == currentUser.Id);

                    return RedirectToAction("CustomerLoginGo", new { id = customerCId.Customer_Guid });
                }
                else
                {
                    ViewBag.UserCustomers = null;

                    return View();
                }

                }
                else
                {
                    ViewBag.UserCustomers = null;

                    return LocalRedirect("~/Home/Index");
                }

    
            }

        public async Task<IActionResult> CustomerLoginGo(string id)
        {
            //save customerguid to appuser field to carry
            var currentUser = await _recitopiaDbContext.AppUsers.Where(m => m.UserName ==  User.Identity.Name).FirstAsync();

            var getCustomerName = await _recitopiaDbContext.Customers.Where(m => m.Customer_Guid == id).FirstAsync();

            currentUser.Customer_Guid = id;
            
            currentUser.Customer_Name = getCustomerName.Customer_Name;
 
            HttpContext.Session.SetString("CurrentUserCustomerGuid", id);

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
                customers.Customer_Guid = Guid.NewGuid().ToString();
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
            
            
            //NEED TO WRITE MULTIPLE CASCADING DELETES TO MAKE SURE THERE ARE NO ORPHANS


            _recitopiaDbContext.Customers.Remove(customers);
            await _recitopiaDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CustomersExists(int id)
        {
            return await _recitopiaDbContext.Customers.AnyAsync(e => e.Customer_Id == id);
        }
        
        public async Task<IActionResult> CopyCustomer(int? id)
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
        [HttpPost, ActionName("CopyCustomer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CopyCustomerConfirmed(int id)
        {
            //

            var customers = await _recitopiaDbContext.Customers.FindAsync(id);
            try
            {    
                Customers customerNew = new Customers()
                {
                    Customer_Name = "Copy of - " + customers.Customer_Name,
                    Phone = customers.Phone,
                    Email = customers.Email,
                    Address1 = customers.Address1,
                    Address2 = customers.Address2,
                    City = customers.City,
                    State = customers.State,
                    Zip = customers.Zip,
                    Web_URL = customers.Web_URL,
                    Notes = customers.Notes
                };
                await _recitopiaDbContext.Customers.AddAsync(customerNew);
                await _recitopiaDbContext.SaveChangesAsync();
                
               

            }
            catch
            {

            }



                return RedirectToAction(nameof(Index));
        }





    }
}
