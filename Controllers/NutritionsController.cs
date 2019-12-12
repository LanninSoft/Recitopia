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

namespace Recitopia.Controllers
{
    public class NutritionsController : AuthorizeController
    {
        private RecitopiaDBContext db = new RecitopiaDBContext();
        [Authorize]
        // GET: Nutritions
        public ActionResult Index()
        {
            var nutritionList = db.Nutrition.OrderBy(o => o.Nutrition_Item).ToList();

            //nutritionList.Sort("Nutrition_Item");

            return View(nutritionList);
        }
        [HttpGet]
        public JsonResult GetData()
        {
            //doesn't allow the model to get dependent table information
            //db.Configuration.ProxyCreationEnabled = false;
            List<Nutrition> nutrition = db.Nutrition.OrderBy(m => m.Nutrition_Item).ToList();
            if (nutrition != null)
            {
                return Json(nutrition);
            }
            return Json(new { Status = "Failure" });
        }
        // GET: Nutritions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Nutrition nutrition = db.Nutrition.Find(id);
            if (nutrition == null)
            {
                return NotFound();
            }
            return View(nutrition);
        }

        // GET: Nutritions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Nutritions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] Nutrition nutrition)
        {
            if (ModelState.IsValid)
            {
                db.Nutrition.Add(nutrition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nutrition);
        }

        // GET: Nutritions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Nutrition nutrition = db.Nutrition.Find(id);
            if (nutrition == null)
            {
                return NotFound();
            }
            return View(nutrition);
        }

        // POST: Nutritions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromForm] Nutrition nutrition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nutrition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nutrition);
        }

        // GET: Nutritions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Nutrition nutrition = db.Nutrition.Find(id);
            if (nutrition == null)
            {
                return NotFound();
            }
            return View(nutrition);
        }

        // POST: Nutritions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Nutrition nutrition = db.Nutrition.Find(id);
           
            try
            {
                db.Nutrition.Remove(nutrition);
                db.SaveChanges();
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
