using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recitopia.Data;
using Recitopia.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Recitopia.Controllers
{
    public class VendorsController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public VendorsController(RecitopiaDBContext recitopiaDbContext)
        {
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
        }

        [Authorize]
        public async Task<ActionResult> Index()
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            var vendors = await _recitopiaDbContext.Vendor
                .Where(m => m.Customer_Guid == customerGuid)
                .OrderBy(m => m.Vendor_Name)
                .ToListAsync();

            return View(vendors);
        }

        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            var vendors = await _recitopiaDbContext.Vendor
                .Where(m => m.Customer_Guid == customerGuid)
                .OrderBy(m => m.Vendor_Name)
                .ToListAsync();

            return Json(vendors);
        }

        public async Task<ActionResult> Details(int id)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
         
            var vendor = await _recitopiaDbContext.Vendor.FindAsync(id);

            if (vendor == null)
            {
                return NotFound();
            }

            return View(vendor);
        }

        public ActionResult Create()
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] Vendor vendor)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (ModelState.IsValid)
            {
                vendor.Customer_Guid = customerGuid;
                _recitopiaDbContext.Vendor.Add(vendor);

                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(vendor);
        }

        public async Task<ActionResult> Edit(int id)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            var vendor = await _recitopiaDbContext.Vendor.FindAsync(id);

            if (vendor == null)
            {
                return NotFound();
            }

            return View(vendor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([FromForm] Vendor vendor)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (ModelState.IsValid)
            {
                vendor.Customer_Guid = customerGuid;
                _recitopiaDbContext.Entry(vendor).State = EntityState.Modified;

                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(vendor);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            var vendor = await _recitopiaDbContext.Vendor.FindAsync(id);

            if (vendor == null)
            {
                return NotFound();
            }

            return View(vendor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var vendor = await _recitopiaDbContext.Vendor.FindAsync(id);

            try
            {
                _recitopiaDbContext.Vendor.Remove(vendor);
                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(vendor);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "This Vendor is associated to an Ingredient and cannot be removed.";
                return View(vendor);
            }
        }
    }
}
