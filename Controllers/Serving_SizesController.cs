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
    public class Serving_SizesController : AuthorizeController
    {
       private RecitopiaDBContext db = new RecitopiaDBContext();
        [Authorize]
        // GET: Serving_Sizes
        public ActionResult Index()
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            return View(db.Serving_Sizes.Where(m => m.Customer_Id == CustomerId).ToList());
        }
        [HttpGet]
        public JsonResult GetData()
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));

            List<Serving_Sizes> SS = db.Serving_Sizes.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Serving_Size).ToList();
            if (SS != null)
            {
                return Json(SS);
            }
            return Json(new { Status = "Failure" });
        }
        // GET: Serving_Sizes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Serving_Sizes serving_Sizes = db.Serving_Sizes.Find(id);
            if (serving_Sizes == null)
            {
                return NotFound();
            }
            return View(serving_Sizes);
        }

        // GET: Serving_Sizes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Serving_Sizes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] Serving_Sizes serving_Sizes)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (ModelState.IsValid)
            {
                serving_Sizes.Customer_Id = CustomerId;
                db.Serving_Sizes.Add(serving_Sizes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(serving_Sizes);
        }

        // GET: Serving_Sizes/Edit/5
        public ActionResult Edit(int? id)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Serving_Sizes serving_Sizes = db.Serving_Sizes.Find(id);
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
        public ActionResult Edit([FromForm] Serving_Sizes serving_Sizes)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (ModelState.IsValid)
            {
                serving_Sizes.Customer_Id = CustomerId;
                db.Entry(serving_Sizes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(serving_Sizes);
        }

        // GET: Serving_Sizes/Delete/5
        public ActionResult Delete(int? id)
        {

            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Serving_Sizes serving_Sizes = db.Serving_Sizes.Find(id);
            if (serving_Sizes == null)
            {
                return NotFound();
            }
            return View(serving_Sizes);
        }

        // POST: Serving_Sizes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Serving_Sizes serving_Sizes = db.Serving_Sizes.Find(id);
            db.Serving_Sizes.Remove(serving_Sizes);
            db.SaveChanges();
            return RedirectToAction("Index");
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
