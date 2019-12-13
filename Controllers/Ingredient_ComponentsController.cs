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
    public class Ingredient_ComponentsController : AuthorizeController
    {
        private RecitopiaDBContext db = new RecitopiaDBContext();
        [Authorize]
        // GET: Ingredient_Components
        public ActionResult Index(int IngredID)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            

            var ingredient_Components = db.Ingredient_Components.Where(m => m.Customer_Id == CustomerId).Include(i => i.Components).Where(m => m.Customer_Id == CustomerId).Include(i => i.Ingredients).Where(m => m.Customer_Id == CustomerId);
            //Filter down to id looking for
            var Ingred_nut = ingredient_Components.Where(i => i.Ingred_Id.Equals(IngredID) && i.Customer_Id == CustomerId);



            //get Ingred name from db and pass in viewbag
            Ingredient ingredient = db.Ingredient.Find(IngredID);

            //Assign to temp local to put on view
            ViewBag.IngredName = ingredient.Ingred_name;

            ViewBag.IngredNameID = IngredID;

            return View(Ingred_nut.ToList());
        }
        [HttpGet]
        public JsonResult GetData(int ingredId)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            //BUILD VIEW FOR ANGULARJS RENDERING
            var query =
               from t1 in db.Ingredient_Components.AsQueryable()
               join t2 in db.Ingredient.AsQueryable() on t1.Ingred_Id equals t2.Ingredient_Id into t2g
               from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t3 in db.Components.AsQueryable() on t1.Comp_Id equals t3.Comp_Id
               where t1.Ingred_Id == ingredId
               where t1.Customer_Id == CustomerId
               orderby t3.Component_Name
               select new View_Angular_Ingredient_Components_Details()
               {
                   Id = t1.Id,
                   Customer_Id = t1.Customer_Id,
                   Ingred_Id = t1.Ingred_Id,
                   Ingred_name = t2.Ingred_name,
                   Comp_Id = t1.Comp_Id,
                   Component_Name = t3.Component_Name,                  

               };

            List<View_Angular_Ingredient_Components_Details> ingredientcomponent = query.ToList();

            if (ingredientcomponent != null)
            {
                return Json(ingredientcomponent);
            }
            return Json(new { Status = "Failure" });
        }
        // GET: Ingredient_Components/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Ingredient_Components ingredient_Components = db.Ingredient_Components.Find(id);
            if (ingredient_Components == null)
            {
                return NotFound();
            }
            return View(ingredient_Components);
        }

        // GET: Ingredient_Components/Create
        public ActionResult Create(int IngredID)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            //get recipe name from db and pass in viewbag
            Ingredient ingredient = db.Ingredient.Find(IngredID);

            //Assign to temp local to put on view
            ViewBag.IngredName = ingredient.Ingred_name;
            ViewBag.IngredId = IngredID;

            ViewBag.Ingred_Id = new SelectList(db.Ingredient.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Ingred_name), "Ingredient_Id", "Ingred_name");
            ViewBag.Comp_Id = new SelectList(db.Components.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Comp_Sort), "Comp_Id", "Component_Name");
            //ViewBag.Comp_Id = new SelectList(db.Components.Select(m => new { m.Comp_Id, m.Component_Name }));


            //BUILD VIEW FOR ALREADY ADDED COMPONENTS
            var query =
               from t1 in db.Ingredient_Components.AsQueryable()
               join t2 in db.Ingredient.AsQueryable() on t1.Ingred_Id equals t2.Ingredient_Id into t2g
               from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t3 in db.Components.AsQueryable() on t1.Comp_Id equals t3.Comp_Id
               where t1.Ingred_Id == IngredID
               where t1.Customer_Id == CustomerId
               orderby t3.Component_Name
               select new View_All_Ingredient_Components()
               {
                   Id = t1.Id,
                   Customer_Id = t1.Customer_Id,
                   Ingred_Id = t1.Ingred_Id,
                   Ingred_name = t2.Ingred_name,
                   Comp_Id = t1.Comp_Id,
                   Component_Name = t3.Component_Name,

               };
            //Get View and filter and sort            
            var TempV = query.ToList();

            //build list to populate previously added items
            List<string> comList = new List<string>();

            foreach (View_All_Ingredient_Components ingred in TempV)
            {
                comList.Add(ingred.Component_Name);
            }

            ViewBag.Components = comList;

            return View();


        }

        // POST: Ingredient_Components/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] Ingredient_Components ingredient_Components, int RID)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (RID > 0)
            {

                ingredient_Components.Ingred_Id = RID;
                ingredient_Components.Customer_Id = CustomerId;
                db.Ingredient_Components.Add(ingredient_Components);
                db.SaveChanges();
                return RedirectToAction("Index", new { IngredID = ingredient_Components.Ingred_Id });
            }

            ViewBag.Comp_Id = new SelectList(db.Components.Where(m => m.Customer_Id == CustomerId), "Comp_Id", "Component_Name", ingredient_Components.Comp_Id);
            ViewBag.Ingred_Id = RID;
            return View(ingredient_Components);
        }

        // GET: Ingredient_Components/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Ingredient_Components ingredient_Components = db.Ingredient_Components.Find(id);
            if (ingredient_Components == null)
            {
                return NotFound();
            }
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            //get name from db and pass in viewbag
            Ingredient ingredient = db.Ingredient.Find(ingredient_Components.Ingred_Id);

            //Assign to temp local to put on view
            ViewBag.IngredName = ingredient.Ingred_name;
            ViewBag.IngredId = ingredient.Ingredient_Id;

            ViewBag.Comp_Id = new SelectList(db.Components.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Comp_Sort), "Comp_Id", "Component_Name", ingredient_Components.Comp_Id);

            ViewBag.Ingred_Id = new SelectList(db.Ingredient.Where(m => m.Customer_Id == CustomerId), "Ingredient_Id", "Ingred_name", ingredient_Components.Ingred_Id);
            return View(ingredient_Components);
        }

        // POST: Ingredient_Components/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromForm] Ingredient_Components ingredient_Components)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (ModelState.IsValid)
            {
                ingredient_Components.Customer_Id = CustomerId;
                db.Entry(ingredient_Components).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { IngredID = ingredient_Components.Ingred_Id });
            }
            
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            ViewBag.Comp_Id = new SelectList(db.Components.Where(m => m.Customer_Id == CustomerId), "Comp_Id", "Component_Name", ingredient_Components.Comp_Id);
            ViewBag.Ingred_Id = new SelectList(db.Ingredient.Where(m => m.Customer_Id == CustomerId), "Ingredient_Id", "Ingred_name", ingredient_Components.Ingred_Id);
            return View(ingredient_Components);
        }

        // GET: Ingredient_Components/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }

            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            var Ingredients_Components = db.Ingredient_Components.Where(m => m.Id == id).Include(r => r.Ingredients).Where(m => m.Customer_Id == CustomerId).Include(r => r.Components).Where(m => m.Customer_Id == CustomerId);

            // var recipe_Ingredients = recipe_Ingredients_Temp.Where(m => m.Id == id);

            if (Ingredients_Components == null)
            {
                return NotFound();
            }
            //GOT TO BE A BETTER WAY THAN BUILD THE MODEL BY HAND:/
            Ingredient_Components rIngreds = new Ingredient_Components();
            Ingredient tIngred = new Ingredient();
            Components tCom = new Components();

            foreach (Ingredient_Components thing in Ingredients_Components)
            {
                tIngred = thing.Ingredients;
                tCom = thing.Components;

                rIngreds.Id = thing.Id;
                rIngreds.Ingred_Id = thing.Ingred_Id;
                rIngreds.Ingredients = tIngred;
                rIngreds.Comp_Id = thing.Comp_Id;
                rIngreds.Components = tCom;

            }


            //Ingredient_Components ingredient_Components = db.Ingredient_Components.Find(id);
            //if (ingredient_Components == null)
            //{
            //    return NotFound();
            //}
            return View(rIngreds);
        }

        // POST: Ingredient_Components/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ingredient_Components ingredient_Components = db.Ingredient_Components.Find(id);

            try
            {
                db.Ingredient_Components.Remove(ingredient_Components);
                db.SaveChanges();
                return RedirectToAction("Index", new { IngredID = ingredient_Components.Ingred_Id });
            }
           
            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(ingredient_Components);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "There was an issue deleting this Nutrient from this Ingredient.";
                return View(ingredient_Components);
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
