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

    public class VendorsController : AuthorizeController
    {
       private RecitopiaDBContext db = new RecitopiaDBContext();
        [Authorize]
        // GET: Vendors
        public ActionResult Index()
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            var vendors = db.Vendor.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Vendor_Name).ToList();
            if (vendors != null)
            {
                

                var tempCheckit = vendors.ToList();

                return View(tempCheckit);
            }
            else
            {
                return View();
            }


        }
        [HttpGet]
        public JsonResult GetData()
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            List<Vendor> vendors = db.Vendor.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Vendor_Name).ToList();
            if (vendors != null)
            {
                return Json(vendors);
            }
            return Json(new { Status = "Failure" });
        }


        // GET: Vendors/Details/5
        public ActionResult Details(int? id)
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
            Vendor vendor = db.Vendor.Find(id);
            if (vendor == null)
            {
                return NotFound();
            }
            return View(vendor);
        }

        // GET: Vendors/Create
        public ActionResult Create()
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            return View();
        }

        // POST: Vendors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] Vendor vendor)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            if (ModelState.IsValid)
            {
                vendor.Customer_Id = CustomerId;
                db.Vendor.Add(vendor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vendor);
        }

        // GET: Vendors/Edit/5
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
            Vendor vendor = db.Vendor.Find(id);
            if (vendor == null)
            {
                return NotFound();
            }
            return View(vendor);
        }

        // POST: Vendors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromForm] Vendor vendor)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (ModelState.IsValid)
            {
                vendor.Customer_Id = CustomerId;
                db.Entry(vendor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vendor);
        }

        // GET: Vendors/Delete/5
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
            Vendor vendor = db.Vendor.Find(id);
            if (vendor == null)
            {
                return NotFound();
            }
            return View(vendor);
        }

        // POST: Vendors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vendor vendor = db.Vendor.Find(id);

            try
            {
                db.Vendor.Remove(vendor);
                db.SaveChanges();
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
