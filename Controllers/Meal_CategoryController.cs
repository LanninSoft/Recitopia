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
    public class Meal_CategoryController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;
        public Meal_CategoryController(RecitopiaDBContext recitopiaDbContext)
        {
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
        }

        [Authorize]
        public async Task<ActionResult> Index()
        {
            var customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));

            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            var mealCategories = await _recitopiaDbContext.Meal_Category
                .Where(m => m.Customer_Id == customerId)
                .ToListAsync();

            return View(mealCategories);
        }

        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            var customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));

            var mealCategories = await _recitopiaDbContext.Meal_Category
                .Where(m => m.Customer_Id == customerId)
                .OrderBy(m => m.Category_Name)
                .ToListAsync();

            return mealCategories != null
                ? Json(mealCategories)
                : Json(new { Status = "Failure" });
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }

            var mealCategory = await _recitopiaDbContext.Meal_Category.FindAsync(id);

            if (mealCategory == null)
            {
                return NotFound();
            }

            return View(mealCategory);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] Meal_Category mealCategory)
        {

            var customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));

            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (ModelState.IsValid)
            {

                mealCategory.Customer_Id = customerId;

                _recitopiaDbContext.Meal_Category.Add(mealCategory);
                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(mealCategory);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var mealCategory = await _recitopiaDbContext.Meal_Category.FindAsync(id);

            if (mealCategory == null)
            {
                return NotFound();
            }

            return View(mealCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([FromForm] Meal_Category mealCategory)
        {
            var customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));

            if (ModelState.IsValid)
            {
                mealCategory.Customer_Id = customerId;

                _recitopiaDbContext.Entry(mealCategory).State = EntityState.Modified;
                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(mealCategory);
        }

        public ActionResult Delete(int id)
        {
            var mealCategory = _recitopiaDbContext.Meal_Category.Find(id);

            if (mealCategory == null)
            {
                return NotFound();
            }

            return View(mealCategory);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var mealCategory = await _recitopiaDbContext.Meal_Category.FindAsync(id);

            try
            {
                _recitopiaDbContext.Meal_Category.Remove(mealCategory);
                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(mealCategory);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "This Meal Category is associated to a Recipe and cannot be deleted.";
                return View(mealCategory);
            }
        }
    }
}
