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
    public class Serving_SizesController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;
        public Serving_SizesController(RecitopiaDBContext recitopiaDbContext)
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

            var servingSizes = await _recitopiaDbContext.Serving_Sizes
                .Where(m => m.Customer_Guid == customerGuid)
                .ToListAsync();

            return View(servingSizes);
        }

        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            var servingSizes = await _recitopiaDbContext.Serving_Sizes
                .Where(m => m.Customer_Guid == customerGuid)
                .OrderBy(m => m.Serving_Size)
                .ToListAsync();

            return servingSizes != null 
                ? Json(servingSizes) 
                : Json(new { Status = "Failure" });
        }

        public async Task<ActionResult> Details(int id)
        {
            var servingSize = await _recitopiaDbContext.Serving_Sizes.FindAsync(id);

            if (servingSize == null)
            {
                return NotFound();
            }

            return View(servingSize);
        }

        public  ActionResult Create()
        {
            return View();
        }

        // POST: Serving_Sizes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] Serving_Sizes serving_Sizes)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");
            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (ModelState.IsValid)
            {
                serving_Sizes.Customer_Guid = customerGuid;
                await _recitopiaDbContext.Serving_Sizes.AddAsync(serving_Sizes);
                await _recitopiaDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(serving_Sizes);
        }

        // GET: Serving_Sizes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");
            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Serving_Sizes serving_Sizes = await _recitopiaDbContext.Serving_Sizes.FindAsync(id);
            if (serving_Sizes == null)
            {
                return NotFound();
            }
            return View(serving_Sizes);
        }

        // POST: Serving_Sizes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([FromForm] Serving_Sizes serving_Sizes)
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");
            if (ModelState.IsValid)
            {
                serving_Sizes.Customer_Guid = customerGuid;
                _recitopiaDbContext.Entry(serving_Sizes).State = EntityState.Modified;
                await _recitopiaDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(serving_Sizes);
        }

        // GET: Serving_Sizes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {

            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");
            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Serving_Sizes serving_Sizes = await _recitopiaDbContext.Serving_Sizes.FindAsync(id);
            if (serving_Sizes == null)
            {
                return NotFound();
            }
            return View(serving_Sizes);
        }

        // POST: Serving_Sizes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Serving_Sizes serving_Sizes = _recitopiaDbContext.Serving_Sizes.Find(id);
            _recitopiaDbContext.Serving_Sizes.Remove(serving_Sizes);
            await _recitopiaDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _recitopiaDbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
