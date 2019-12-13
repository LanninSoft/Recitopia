﻿using Microsoft.AspNetCore.Mvc;
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
    public class ComponentsController : AuthorizeController
    {
        private RecitopiaDBContext db = new RecitopiaDBContext();
        [Authorize]
        // GET: Components
        public ActionResult Index()
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            List<Components> TempV = db.Components.Where(m => m.Customer_Id == CustomerId).ToList();

            var sortedList = TempV.OrderBy(m => m.Comp_Sort).ToList();

            return View(sortedList);
        }
        [HttpGet]
        public JsonResult GetData()
        {
            //doesn't allow the model to get dependent table information
            //db.Configuration.ProxyCreationEnabled = false;
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            

            List<Components> allergen = db.Components.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Component_Name).ToList();
            if (allergen != null)
            {
                return Json(allergen);
            }
            return Json(new { Status = "Failure" });
        }
        // GET: Components/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Components component = db.Components.Find(id);
            if (component == null)
            {
                return NotFound();
            }
            return View(component);
        }

        // GET: Components/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Components/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] Components component)
        {
            if (ModelState.IsValid)
            {
                int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
                if (CustomerId == 0)
                {
                    return RedirectToAction("CustomerLogin", "Customers");
                }
               
                var compFullName = component.Component_Name.ToString();

                char[] arr = compFullName.ToCharArray();

                arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c)
                                                  || char.IsWhiteSpace(c)
                                                  || c == '-')));
                var compFN = new string(arr);
                var intStrLen = compFN.Length;

                if (intStrLen > 48)
                {
                    component.Comp_Sort = compFN.Substring(0, 48);
                }
                else
                {
                    component.Comp_Sort = compFN.Substring(0, intStrLen);
                }

                //---------------------------------------
                component.Customer_Id = CustomerId;

                db.Components.Add(component);

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(component);
        }

        // GET: Components/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Components component = db.Components.Find(id);
            if (component == null)
            {
                return NotFound();
            }
            return View(component);
        }

        // POST: Components/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromForm] Components component)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
           
            if (ModelState.IsValid)
            {
                //Remove all extra and save 48 characters to Comp_sort field
                var compFullName = component.Component_Name.ToString();

                char[] arr = compFullName.ToCharArray();

                arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c)
                                                  || char.IsWhiteSpace(c)
                                                  || c == '-')));
                var compFN = new string(arr);

                var intStrLen = compFN.Length;

                if (intStrLen > 48)
                {
                    component.Comp_Sort = compFN.Substring(0, 48);
                }
                else
                {
                    component.Comp_Sort = compFN.Substring(0, intStrLen);
                }

                //---------------------------------------
                component.Customer_Id = CustomerId;
                db.Entry(component).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(component);
        }

        // GET: Components/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Components component = db.Components.Find(id);
            if (component == null)
            {
                return NotFound();
            }
            return View(component);
        }

        // POST: Components/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Components component = db.Components.Find(id);


            try
            {


                db.Components.Remove(component);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            
            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(component);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 3";
                return View(component);
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
