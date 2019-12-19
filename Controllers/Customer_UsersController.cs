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
        public async Task<IActionResult> Index()
        {
            //BUILD VIEW FOR ANGULARJS RENDERING
            //For some reason, I am unable to recreate this quagmire in LINQ format.  most likely to do with Models
            var customerUsers = await _recitopiaDbContext.Customer_Users
                           .Include(ri => ri.AppUser)
                           .Include(ri => ri.Customers)
                           .OrderBy(ri => ri.Customers.Customer_Name)
                           .Select(ri => new Customer_Users()
                           {
                               CU_Id = ri.CU_Id,
                               Id = ri.Id,
                               Customer_Id = ri.Customer_Id,
                               Notes = ri.Notes,
                               Customer_Name = ri.Customer_Name,
                               User_Name = ri.AppUser.FirstName + ' ' + ri.AppUser.LastName
                           }).ToListAsync();

            //var query =
            //   from t1 in _recitopiaDbContext.Customer_Users.AsQueryable()
            //   join t2 in _recitopiaDbContext.AppUsers.AsQueryable() on t1.Id equals t2.Id into t2g
            //   from t2 in t2g.DefaultIfEmpty()
            //   join t3 in _recitopiaDbContext.Customers.AsQueryable() on t1.Customer_Id equals t3.Customer_Id into t3g
            //   from t3 in t3g.DefaultIfEmpty()
            //   select new Customer_Users()
            //   {
            //       CU_Id = t1.CU_Id,
            //       Id = t1.Id,
            //       Customer_Id = t1.Customer_Id,
            //       Notes = t1.Notes,
            //       Customer_Name = t3.Customer_Name,
            //       User_Name = t2.FirstName + ' ' + t2.LastName
            //   };

            return View(customerUsers);
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
               join t2 in _recitopiaDbContext.AppUsers.AsQueryable() on t1.Id equals t2.Id into t2g
               from t2 in t2g.DefaultIfEmpty()
               join t3 in _recitopiaDbContext.Customers.AsQueryable() on t1.Customer_Id equals t3.Customer_Id into t3g
               from t3 in t3g.DefaultIfEmpty()
               select new Customer_Users()
               {
                   CU_Id = t1.CU_Id,
                   Id = t1.Id,
                   Customer_Id = t1.Customer_Id,
                   Notes = t1.Notes,
                   Customer_Name = t3.Customer_Name,
                   User_Name = t2.FirstName + ' ' + t2.LastName
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
               .FirstOrDefaultAsync(m => m.CU_Id == id);

           if (customer_Users == null)
            {
                return NotFound();
            }

            return View(customer_Users);
        }

        // GET: Customer_Users/Create
        public async Task<IActionResult> Create()
        {

           
            IEnumerable<SelectListItem> appUserCustomers = await _recitopiaDbContext.Customers.Select(c => new SelectListItem
            {
                Value = c.Customer_Id.ToString(),
                Text = c.Customer_Name

            }).ToListAsync();

            ViewBag.Customers = appUserCustomers;

            IEnumerable<SelectListItem> appUserList = await _recitopiaDbContext.AppUsers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FirstName + ' ' + c.LastName

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
                _recitopiaDbContext.Add(customer_Users);
                await _recitopiaDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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

            var customer_Users = await _recitopiaDbContext.Customer_Users.FindAsync(id);

            if (customer_Users == null)
            {
                return NotFound();
            }

            
            IEnumerable<SelectListItem> appUserCustomers = await _recitopiaDbContext.Customers.Select(c => new SelectListItem
            {
                Value = c.Customer_Id.ToString(),
                Text = c.Customer_Name
            }).ToListAsync();

            ViewBag.Customers = appUserCustomers;

           
            IEnumerable<SelectListItem> appUserList = await _recitopiaDbContext.AppUsers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FirstName + ' ' + c.LastName
            }).ToListAsync();

            ViewBag.AppUsers = appUserList;

            return View(customer_Users);
        }

        // POST: Customer_Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Customer_Users customer_Users)
        {
           
            if (customer_Users.CU_Id != 0)
            {
                try
                {
                    _recitopiaDbContext.Update(customer_Users);
                    await _recitopiaDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await Customer_UsersExists(customer_Users.CU_Id))
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

            var customer_Users = await _recitopiaDbContext.Customer_Users
                .FirstOrDefaultAsync(m => m.CU_Id == id);
            if (customer_Users == null)
            {
                return NotFound();
            }

            return View(customer_Users);
        }

        // POST: Customer_Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer_Users = await _recitopiaDbContext.Customer_Users.FindAsync(id);
            _recitopiaDbContext.Customer_Users.Remove(customer_Users);
            await _recitopiaDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> Customer_UsersExists(int id)
        {
            return await _recitopiaDbContext.Customer_Users.AnyAsync(e => e.CU_Id == id);
        }
    }
}
