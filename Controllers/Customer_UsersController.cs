using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recitopia.Models;

namespace Recitopia.Controllers
{
    public class Customer_UsersController : Controller
    {
        private readonly RecitopiaDBContext db;

        public Customer_UsersController(RecitopiaDBContext context)
        {
            db = context;



        }
        [Authorize]
        // GET: Customer_Users
        public async Task<IActionResult> Index()
        {


            //BUILD VIEW FOR ANGULARJS RENDERING
            var query =
               from t1 in db.Customer_Users.AsQueryable()
               join t2 in db.AppUsers.AsQueryable() on t1.Id equals t2.Id into t2g
               from t2 in t2g.DefaultIfEmpty()
               join t3 in db.Customers.AsQueryable() on t1.Customer_Id equals t3.Customer_Id into t3g
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

            return View(await query.ToListAsync());


        }

        // GET: Customer_Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer_Users = await db.Customer_Users
                .FirstOrDefaultAsync(m => m.CU_Id == id);
            if (customer_Users == null)
            {
                return NotFound();
            }

            return View(customer_Users);
        }

        // GET: Customer_Users/Create
        public IActionResult Create()
        {

            var Customers = db.Customers;

            IEnumerable<SelectListItem> appUserCustomers = db.Customers.Select(c => new SelectListItem
            {
                Value = c.Customer_Id.ToString(),
                Text = c.Customer_Name

            });
            ViewBag.Customers = appUserCustomers;

            var appUsers = db.AppUsers;

            IEnumerable<SelectListItem> appUserList = db.AppUsers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FirstName + ' ' + c.LastName

            });
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
                db.Add(customer_Users);
                await db.SaveChangesAsync();
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

            var customer_Users = await db.Customer_Users.FindAsync(id);

            if (customer_Users == null)
            {
                return NotFound();
            }

            var Customers = db.Customers;

            IEnumerable<SelectListItem> appUserCustomers = db.Customers.Select(c => new SelectListItem
            {
                Value = c.Customer_Id.ToString(),
                Text = c.Customer_Name
            });

            ViewBag.Customers = appUserCustomers;

            var appUsers = db.AppUsers;

            IEnumerable<SelectListItem> appUserList = db.AppUsers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FirstName + ' ' + c.LastName
            });

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
                    db.Update(customer_Users);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Customer_UsersExists(customer_Users.CU_Id))
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

            var customer_Users = await db.Customer_Users
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
            var customer_Users = await db.Customer_Users.FindAsync(id);
            db.Customer_Users.Remove(customer_Users);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Customer_UsersExists(int id)
        {
            return db.Customer_Users.Any(e => e.CU_Id == id);
        }
    }
}
