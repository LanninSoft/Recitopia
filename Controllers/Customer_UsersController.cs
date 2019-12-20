using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recitopia.Data;
using Recitopia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recitopia.Controllers
{
    public class Customer_UsersController : Controller
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public Customer_UsersController(RecitopiaDBContext recitopiaDbContext)
        {
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
        }

        [Authorize(Roles = "Administrator")]

        // GET: Customer_Users
        public IActionResult Index()
        {
            //BUILD VIEW FOR ANGULARJS RENDERING
            //For some reason, I am unable to recreate this quagmire in LINQ format.  most likely to do with Models
            //var customerUsers = await _recitopiaDbContext.Customer_Users
            //               .Include(ri => ri.AppUser)
            //               .Include(ri => ri.Customers)
            //               .OrderBy(ri => ri.Customers.Customer_Name)
            //               .Select(ri => new Customer_Users()
            //               {
            //                   CU_Id = ri.CU_Id,
            //                   Id = ri.Id,
            //                   Customer_Id = ri.Customer_Id,
            //                   Notes = ri.Notes,
            //                   Customer_Name = ri.Customer_Name,
            //                   User_Name = ri.AppUser.FirstName + ' ' + ri.AppUser.LastName
            //               }).ToListAsync();

            var query = 
               from t1 in _recitopiaDbContext.Customer_Users.AsQueryable()
               join t2 in _recitopiaDbContext.AppUsers.AsQueryable() on t1.User_Id equals t2.Id into t2g
               from t2 in t2g.DefaultIfEmpty()
               join t3 in _recitopiaDbContext.Customers.AsQueryable() on t1.Customer_Id equals t3.Customer_Id into t3g
               from t3 in t3g.DefaultIfEmpty()
               orderby t3.Customer_Name
               select new Customer_Users()
               {
                   Id = t1.Id,
                   User_Id = t1.User_Id,
                   Customer_Id = t1.Customer_Id,
                   Notes = t1.Notes,
                   Customer_Name = t3.Customer_Name,
                   User_Name = t2.FullName
               };

            return View(query.ToList());
        }

        [HttpGet]
        public JsonResult GetData()
        {
            //BUILD VIEW FOR ANGULARJS RENDERING
            //For some reason, I am unable to recreate this quagmire in LINQ format.  most likely to do with Models



            //var customerUsers =  _recitopiaDbContext.Customer_Users
            //               .Include(ri => ri.AppUser)
            //               .Include(ri => ri.Customers)
            //               .OrderBy(ri => ri.Customers.Customer_Name)
            //               .Select(ri => new Customer_Users()
            //               {
            //                   CU_Id = ri.CU_Id,
            //                   Id = ri.Id,
            //                   Customer_Id = ri.Customer_Id,
            //                   Notes = ri.Notes,
            //                   Customer_Name = ri.Customer_Name,
            //                   User_Name = ri.AppUser.FirstName + ' ' + ri.AppUser.LastName
            //               }).ToList();

            var query =
               from t1 in _recitopiaDbContext.Customer_Users.AsQueryable()
               join t2 in _recitopiaDbContext.AppUsers.AsQueryable() on t1.User_Id equals t2.Id into t2g
               from t2 in t2g.DefaultIfEmpty()
               join t3 in _recitopiaDbContext.Customers.AsQueryable() on t1.Customer_Id equals t3.Customer_Id into t3g
               from t3 in t3g.DefaultIfEmpty()
               orderby t3.Customer_Name
               select new Customer_Users()
               {
                   Id = t1.Id,
                   User_Id = t1.User_Id,
                   Customer_Id = t1.Customer_Id,
                   Notes = t1.Notes,
                   Customer_Name = t3.Customer_Name,
                   User_Name = t2.FullName
               };

            List<Customer_Users> customerusers = query.ToList();

            if (customerusers != null)
            {
                return Json(customerusers);
            }
            return Json(new { Status = "Failure" });
        }

        // GET: Customer_Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer_Users = await _recitopiaDbContext.Customer_Users
               .FirstOrDefaultAsync(m => m.Id == id);

           if (customer_Users == null)
            {
                return NotFound();
            }

            return View(customer_Users);
        }

        // GET: Customer_Users/Create
        public async Task<IActionResult> Create()
        {

           
            IEnumerable<SelectListItem> appUserCustomers = await _recitopiaDbContext.Customers.OrderBy(c => c.Customer_Name).Select(c => new SelectListItem
            {
                Value = c.Customer_Id.ToString(),
                Text = c.Customer_Name

            }).ToListAsync();

            ViewBag.Customers = appUserCustomers;

            IEnumerable<SelectListItem> appUserList = await _recitopiaDbContext.AppUsers.OrderBy(c => c.LastName).Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = (c.LastName + ", " + c.FirstName).ToString()

            }).ToListAsync();
            ViewBag.AppUsers = appUserList;


            return View();
        }

        // POST: Customer_Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Customer_Users customer_Users)
        {
            if (ModelState.IsValid)
            {

                //Check if user customer relationship exists already.  if so, let user know they cannot add
                var checkExistence = await _recitopiaDbContext.Customer_Users.Where(m => m.Customer_Id == customer_Users.Customer_Id && m.User_Id == customer_Users.User_Id).SingleOrDefaultAsync();

                if (checkExistence != null)
                {
                    ViewBag.ErrorMessage = "This User is already assigned to this Customer. (" + checkExistence.User_Name +"/" + checkExistence.Customer_Name + ")";

                    IEnumerable<SelectListItem> appUserCustomers = await _recitopiaDbContext.Customers.OrderBy(c => c.Customer_Name).Select(c => new SelectListItem
                    {
                        Value = c.Customer_Id.ToString(),
                        Text = c.Customer_Name

                    }).ToListAsync();

                    ViewBag.Customers = appUserCustomers;

                    IEnumerable<SelectListItem> appUserList = await _recitopiaDbContext.AppUsers.OrderBy(c => c.LastName).Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = (c.LastName + ", " + c.FirstName).ToString()

                    }).ToListAsync();
                    ViewBag.AppUsers = appUserList;

                    return View(customer_Users);
                }
                else
                {
                    var addedUser = await _recitopiaDbContext.AppUsers.SingleAsync(m => m.Id == customer_Users.User_Id);
                    var addedCustomer = await _recitopiaDbContext.Customers.SingleAsync(m => m.Customer_Id == customer_Users.Customer_Id);

                    customer_Users.Customer_Name = addedCustomer.Customer_Name;
                    customer_Users.User_Name = addedUser.FullName;

                    _recitopiaDbContext.Add(customer_Users);

                    await _recitopiaDbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                
                
            }
            return View(customer_Users);
        }

        // GET: Customer_Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query =
               from t1 in _recitopiaDbContext.Customer_Users.AsQueryable()
               join t2 in _recitopiaDbContext.AppUsers.AsQueryable() on t1.User_Id equals t2.Id into t2g
               from t2 in t2g.DefaultIfEmpty()
               join t3 in _recitopiaDbContext.Customers.AsQueryable() on t1.Customer_Id equals t3.Customer_Id into t3g
               from t3 in t3g.DefaultIfEmpty()
               where t1.Id == id
               select new Customer_Users()
               {
                   Id = t1.Id,
                   User_Id = t1.User_Id,
                   Customer_Id = t1.Customer_Id,
                   Notes = t1.Notes,
                   Customer_Name = t3.Customer_Name,
                   User_Name = t2.FirstName + ' ' + t2.LastName
               };

            ViewBag.AppUsers = await query.SingleAsync();

            var customer_Users = await _recitopiaDbContext.Customer_Users
                .SingleAsync(m => m.Id == id);



            if (customer_Users == null)
            {
                return NotFound();
            }

            ViewBag.Customers = new SelectList(await _recitopiaDbContext.Customers
                .OrderBy(m => m.Customer_Name)
                .ToListAsync(), "Customer_Id", "Customer_Name");

            

            return View(customer_Users);
        }

        // POST: Customer_Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Customer_Users customer_Users)
        {
           
            if (customer_Users.Id != 0)
            {
                try
                {
                    //get and set customer name for new choice
                    var customerName = await _recitopiaDbContext.Customers.Where(m => m.Customer_Id == customer_Users.Customer_Id).SingleAsync();

                    customer_Users.Customer_Name = customerName.Customer_Name;

                    _recitopiaDbContext.Update(customer_Users);

                    await _recitopiaDbContext.SaveChangesAsync();

                    //check to see if this created a duplicate entry, if so, remove the duplicate.  There might be a better way here.
                    var checkDuplicates = await _recitopiaDbContext.Customer_Users
                        .Where(m => m.Customer_Id == customer_Users.Customer_Id && m.User_Id == customer_Users.User_Id)
                        .ToListAsync();
                    if (checkDuplicates.Count() > 1)
                    {
                        var countIt = 1;
                        foreach (Customer_Users customerUser in checkDuplicates)
                        {
                            //ignore first pass to keep first record
                            if (countIt > 1)
                            {
                                var recordToDelete = await _recitopiaDbContext.Customer_Users.FindAsync(customerUser.Id);

                                _recitopiaDbContext.Remove(recordToDelete);

                                await _recitopiaDbContext.SaveChangesAsync();

                            }

                            countIt += 1;
                        }

                    }


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await Customer_UsersExists(customer_Users.Id))
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




            return View(customer_Users);
        }

        // GET: Customer_Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //var customer_Users =
            //   from t1 in _recitopiaDbContext.Customer_Users.AsQueryable()
            //   join t2 in _recitopiaDbContext.AppUsers.AsQueryable() on t1.Id equals t2.Id into t2g
            //   from t2 in t2g.DefaultIfEmpty()
            //   join t3 in _recitopiaDbContext.Customers.AsQueryable() on t1.Customer_Id equals t3.Customer_Id into t3g
            //   from t3 in t3g.DefaultIfEmpty()
            //   where t1.CU_Id == id
            //   select new Customer_Users()
            //   {
            //       CU_Id = t1.CU_Id,
            //       Id = t1.Id,
            //       Customer_Id = t1.Customer_Id,
            //       Notes = t1.Notes,
            //       Customer_Name = t3.Customer_Name,
            //       User_Name = t2.FirstName + ' ' + t2.LastName
            //   };
            var customer_Users = await _recitopiaDbContext.Customer_Users.SingleAsync(m => m.Id == id);
            if (customer_Users == null)
            {
                return NotFound();
            }

            return View(customer_Users);
        }

        // POST: Customer_Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, [FromForm] Customer_Users customer_Users)
        {
            //var customer_User = await _recitopiaDbContext.Customer_Users.SingleAsync(m => m.CU_Id == id);

            var custUser = await _recitopiaDbContext.Customer_Users.SingleAsync(m => m.Id == customer_Users.Id);

            _recitopiaDbContext.Customer_Users.Remove(custUser);

            await _recitopiaDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> Customer_UsersExists(int id)
        {
            return await _recitopiaDbContext.Customer_Users.AnyAsync(e => e.Id == id);
        }
    }
}
