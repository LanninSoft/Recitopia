
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using Recitopia.Data;
using Recitopia.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Recitopia.Controllers
{
    public class PackagingController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public PackagingController(RecitopiaDBContext recitopiaDbContext)
        {
            _recitopiaDbContext = recitopiaDbContext;
        }
        [Authorize]
        // GET: Packaging
        public async Task<IActionResult> Index()
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            var packaging = await _recitopiaDbContext.Packaging
                .Include(r => r.Vendor)
                .Where(r => r.Customer_Guid == customerGuid)
                .OrderBy(r => r.Package_Name)
                .ToListAsync();

            return packaging != null ? View(packaging) : View();

            
        }
        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            //BUILD VIEW FOR ANGULARJS RENDERING
            var packaging = await _recitopiaDbContext.Packaging
                .Include(r => r.Vendor)
                .Where(r => r.Customer_Guid == customerGuid)
                .OrderBy(r => r.Package_Name)
                .ToListAsync();


            return packaging != null
                 ? Json(packaging)
                 : Json(new { Status = "Failure" });


        }
        // GET: Packaging/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var packaging = await _recitopiaDbContext.Packaging
                .FirstOrDefaultAsync(m => m.Package_Id == id);
            if (packaging == null)
            {
                return NotFound();
            }

            return View(packaging);
        }

        // GET: Packaging/Create
        public async Task<IActionResult> Create()
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            ViewBag.Vendor_Id = new SelectList(await _recitopiaDbContext.Vendor.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Vendor_Name).ToListAsync(), "Vendor_Id", "Vendor_Name");

            return View();
        }

        // POST: Packaging/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Packaging packaging)
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (ModelState.IsValid)
            {
                var vendorName = await _recitopiaDbContext.Vendor.FindAsync(packaging.Vendor_Id);
                
                packaging.Customer_Guid = customerGuid;
                packaging.Vendor_Name = vendorName.Vendor_Name;
                packaging.LastModified = DateTime.UtcNow;

                _recitopiaDbContext.Add(packaging);
                await _recitopiaDbContext.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }
            ViewBag.Vendor_Id = new SelectList(await _recitopiaDbContext.Vendor.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Vendor_Name).ToListAsync(), "Vendor_Id", "Vendor_Name");

            return View(packaging);
        }

        // GET: Packaging/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (id == null)
            {
                return NotFound();
            }

            var packaging = await _recitopiaDbContext.Packaging.FindAsync(id);

            if (packaging == null)
            {
                return NotFound();
            }
            ViewBag.Package_Name = packaging.Package_Name;
            ViewBag.Vendor_Id = new SelectList(await _recitopiaDbContext.Vendor.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Vendor_Name).ToListAsync(), "Vendor_Id", "Vendor_Name");

            return View(packaging);
        }

        // POST: Packaging/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( [FromForm] Packaging packaging)
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

           
            if (ModelState.IsValid)
            {
                try
                {
                    var vendorName = await _recitopiaDbContext.Vendor.FindAsync(packaging.Vendor_Id);

                    packaging.Customer_Guid = customerGuid;
                    packaging.Vendor_Name = vendorName.Vendor_Name;
                    packaging.LastModified = DateTime.UtcNow;

                    _recitopiaDbContext.Update(packaging);
                    await _recitopiaDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackagingExists(packaging.Package_Id))
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

            
            ViewBag.Package_Name = packaging.Package_Name;
            ViewBag.Vendor_Id = new SelectList(await _recitopiaDbContext.Vendor.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Vendor_Name).ToListAsync(), "Vendor_Id", "Vendor_Name");
            return View(packaging);
        }

        // GET: Packaging/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            var packaging = await _recitopiaDbContext.Packaging
                .FirstOrDefaultAsync(m => m.Package_Id == id);
            if (packaging == null)
            {
                return NotFound();
            }

            return View(packaging);
        }

        // POST: Packaging/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var packaging = await _recitopiaDbContext.Packaging.FindAsync(id);

            try
            {
                
                _recitopiaDbContext.Packaging.Remove(packaging);
                await _recitopiaDbContext.SaveChangesAsync();

                //remove recipe packaging
                //GET RECIPE PACKAGING
                var recipePackaging = await _recitopiaDbContext.Recipe_Packaging.Where(m => m.Package_Id == id).ToListAsync();

                _recitopiaDbContext.Recipe_Packaging.RemoveRange(recipePackaging);

                await _recitopiaDbContext.SaveChangesAsync();
               
                return RedirectToAction(nameof(Index));
            }

            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(packaging);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "There are associated ingredients that need to be removed before you can delete this recipe.";
                return View(packaging);
            }
        }

        private bool PackagingExists(int id)
        {
            return _recitopiaDbContext.Packaging.Any(e => e.Package_Id == id);
        }
    }
}
