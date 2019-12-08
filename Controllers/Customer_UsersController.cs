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
        private readonly RecitopiaDBContext _context;

        public Customer_UsersController(RecitopiaDBContext context)
        {
            _context = context;
        }
        [Authorize]
        // GET: Customer_Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customer_Users.ToListAsync());
        }

        // GET: Customer_Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer_Users = await _context.Customer_Users
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
            return View();
        }

        // POST: Customer_Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CU_Id,Customer_Id,Id,Notes")] Customer_Users customer_Users)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer_Users);
                await _context.SaveChangesAsync();
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

            var customer_Users = await _context.Customer_Users.FindAsync(id);
            if (customer_Users == null)
            {
                return NotFound();
            }
            return View(customer_Users);
        }

        // POST: Customer_Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CU_Id,Customer_Id,Id,Notes")] Customer_Users customer_Users)
        {
            if (id != customer_Users.CU_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer_Users);
                    await _context.SaveChangesAsync();
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

            var customer_Users = await _context.Customer_Users
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
            var customer_Users = await _context.Customer_Users.FindAsync(id);
            _context.Customer_Users.Remove(customer_Users);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Customer_UsersExists(int id)
        {
            return _context.Customer_Users.Any(e => e.CU_Id == id);
        }
    }
}
