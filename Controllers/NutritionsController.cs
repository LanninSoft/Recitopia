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
    public class NutritionsController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public NutritionsController(RecitopiaDBContext recitopiaDbContext)
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

            var nutritionList = await _recitopiaDbContext.Nutrition
                .Where(m => m.Customer_Guid == customerGuid)
                .OrderBy(o => o.Nutrition_Item)
                .ToListAsync();

            return View(nutritionList);
        }

        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            var nutritions = await _recitopiaDbContext.Nutrition
                .Where(m => m.Customer_Guid == customerGuid)
                .OrderBy(m => m.Nutrition_Item)
                .ToListAsync();

            return nutritions != null 
                ? Json(nutritions) 
                : Json(new { Status = "Failure" });

        }

        public async Task<ActionResult> Details(int id)
        {
            var nutrition = await _recitopiaDbContext.Nutrition.FindAsync(id);

            if (nutrition == null)
            {
                return NotFound();
            }

            return View(nutrition);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] Nutrition nutrition)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (ModelState.IsValid)
            {
                nutrition.Customer_Guid = customerGuid;
                _recitopiaDbContext.Nutrition.Add(nutrition);
                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(nutrition);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var nutrition = await _recitopiaDbContext.Nutrition.FindAsync(id);

            if (nutrition == null)
            {
                return NotFound();
            }

            return View(nutrition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([FromForm] Nutrition nutrition)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (ModelState.IsValid)
            {
                nutrition.Customer_Guid = customerGuid;
                _recitopiaDbContext.Entry(nutrition).State = EntityState.Modified;
                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(nutrition);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var nutrition = await _recitopiaDbContext.Nutrition.FindAsync(id);

            if (nutrition == null)
            {
                return NotFound();
            }

            return View(nutrition);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var nutrition = await _recitopiaDbContext.Nutrition.FindAsync(id);
           
            try
            {
                _recitopiaDbContext.Nutrition.Remove(nutrition);
                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            
            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(nutrition);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "This Nutrition Item is associated with Ingredient(s) and cannot be deleted.";
                return View(nutrition);
            }
        }
    }
}
