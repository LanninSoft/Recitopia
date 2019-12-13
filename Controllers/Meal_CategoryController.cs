using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Recitopia.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Recitopia.Controllers
{
    public class Meal_CategoryController : AuthorizeController
    {
       private RecitopiaDBContext db = new RecitopiaDBContext();
        [Authorize]
        // GET: Meal_Category
        public ActionResult Index()
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            return View(db.Meal_Category.Where(m => m.Customer_Id == CustomerId).ToList());
        }
        [HttpGet]
        public JsonResult GetData()
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));

            List<Meal_Category> mealC = db.Meal_Category.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Category_Name).ToList();
            if (mealC != null)
            {
                return Json(mealC);
            }
            return Json(new { Status = "Failure" });
        }
        // GET: Meal_Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Meal_Category meal_Category = db.Meal_Category.Find(id);
            if (meal_Category == null)
            {
                return NotFound();
            }
            return View(meal_Category);
        }

        // GET: Meal_Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Meal_Category/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] Meal_Category meal_Category)
        {

            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (ModelState.IsValid)
            {

                meal_Category.Customer_Id = CustomerId;

                db.Meal_Category.Add(meal_Category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(meal_Category);
        }

        // GET: Meal_Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Meal_Category meal_Category = db.Meal_Category.Find(id);
            if (meal_Category == null)
            {
                return NotFound();
            }
            return View(meal_Category);
        }

        // POST: Meal_Category/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromForm] Meal_Category meal_Category)
        {

            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (ModelState.IsValid)
            {
                meal_Category.Customer_Id = CustomerId;

                db.Entry(meal_Category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(meal_Category);
        }

        // GET: Meal_Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Meal_Category meal_Category = db.Meal_Category.Find(id);
            if (meal_Category == null)
            {
                return NotFound();
            }
            return View(meal_Category);
        }

        // POST: Meal_Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Meal_Category meal_Category = db.Meal_Category.Find(id);

            try
            {
                db.Meal_Category.Remove(meal_Category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(meal_Category);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "This Meal Category is associated to a Recipe and cannot be deleted.";
                return View(meal_Category);
            }



        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
