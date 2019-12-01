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
    public class Ingredient_NutrientsController : AuthorizeController
    {
        private RecitopiaDBContext db = new RecitopiaDBContext();
        [Authorize]
        // GET: Ingredient_Nutrients
        public ActionResult Index(int IngredID)
        {
            var ingredient_Nutrients = db.Ingredient_Nutrients.Include(i => i.Ingredients).Include(i => i.Nutrition);

            //Filter down to id looking for
            var Ingred_nut = ingredient_Nutrients.Where(i => i.Ingred_Id.Equals(IngredID));

            //Sort
            var Ingred_nut2 = Ingred_nut.OrderBy(i => i.Nutrition.OrderOnNutrientPanel);

            //get Ingred name from db and pass in viewbag
            Ingredient ingredient = db.Ingredient.Find(IngredID);

            //Assign to temp local to put on view
            ViewBag.IngredName = ingredient.Ingred_name;

            ViewBag.IngredNameID = IngredID;

            return View(Ingred_nut2.ToList());
        }

        // GET: Ingredient_Nutrients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Ingredient_Nutrients ingredient_Nutrients = db.Ingredient_Nutrients.Find(id);
            if (ingredient_Nutrients == null)
            {
                return NotFound();
            }
            return View(ingredient_Nutrients);
        }

        // GET: Ingredient_Nutrients/Create
        public ActionResult Create(int IngredID)
        {
            //get recipe name from db and pass in viewbag
            Ingredient ingredient = db.Ingredient.Find(IngredID);

            //Assign to temp local to put on view
            ViewBag.IngredName = ingredient.Ingred_name;
            ViewBag.IngredId = IngredID;

            ViewBag.Ingred_Id = new SelectList(db.Ingredient.OrderBy(m => m.Ingred_name), "Ingredient_Id", "Ingred_name");
            ViewBag.Nutrition_Item_Id = new SelectList(db.Nutrition.OrderBy(m => m.Nutrition_Item), "Nutrition_Item_Id", "Nutrition_Item");


            //BUILD VIEW FOR ALREADY ADDED NUTRIENTS
            var query =
               from t1 in db.Ingredient_Nutrients.AsQueryable()
               join t2 in db.Ingredient.AsQueryable() on t1.Ingred_Id equals t2.Ingredient_Id into t2g
               from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t3 in db.Nutrition.AsQueryable() on t1.Nutrition_Item_Id equals t3.Nutrition_Item_Id
               where t1.Ingred_Id == IngredID
               orderby t3.Nutrition_Item
               select new View_All_Ingredient_Nutrients()
               {
                   Id = t1.Id,
                   Customer_Id = t1.Customer_Id,
                   Ingred_Id = t1.Ingred_Id,
                   Ingred_name = t2.Ingred_name,
                   Nutrition_Item_Id = t1.Nutrition_Item_Id,
                   Nutrition_Item = t3.Nutrition_Item,
                   Nut_per_100_grams = t1.Nut_per_100_grams,
                   

               };
            //Get View and filter and sort            
            var TempV = query.ToList();

            //build list to populate previously added items
            List<string> ingList = new List<string>();

            foreach (View_All_Ingredient_Nutrients ingred in TempV)
            {
                ingList.Add(ingred.Nutrition_Item + "/" + ingred.Nut_per_100_grams.ToString() + " Nutrition/100g");
            }

            ViewBag.Nutrition = ingList;

            return View();
        }

        // POST: Ingredient_Nutrients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] Ingredient_Nutrients ingredient_Nutrients, int RID)
        {
            if (RID > 0)
            {
                ViewBag.Ingred_Id = RID;
                ingredient_Nutrients.Ingred_Id = RID;
                
                db.Ingredient_Nutrients.Add(ingredient_Nutrients);
                db.SaveChanges();
                return RedirectToAction("Index", new { IngredID = ingredient_Nutrients.Ingred_Id });
            }

            ViewBag.Ingred_Id = RID;
            ViewBag.Nutrition_Item_Id = new SelectList(db.Nutrition.OrderBy(m => m.Nutrition_Item), "Nutrition_Item_Id", "Nutrition_Item", ingredient_Nutrients.Nutrition_Item_Id);
            return View(ingredient_Nutrients);
        }

        // GET: Ingredient_Nutrients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            //get ingred id
            Ingredient_Nutrients ingredient_nutrients = db.Ingredient_Nutrients.Find(id);
            if (ingredient_nutrients == null)
            {
                return NotFound();
            }
            //get name from db and pass in viewbag
            Ingredient ingredient = db.Ingredient.Find(ingredient_nutrients.Ingred_Id);

            //Assign to temp local to put on view
            ViewBag.IngredName = ingredient.Ingred_name;
            ViewBag.IngredId = ingredient.Ingredient_Id;



            ViewBag.Ingred_Id = new SelectList(db.Ingredient.OrderBy(m => m.Ingred_name), "Ingredient_Id", "Ingred_name", ingredient_nutrients.Ingred_Id);
            ViewBag.Nutrition_Item_Id = new SelectList(db.Nutrition.OrderBy(m => m.Nutrition_Item), "Nutrition_Item_Id", "Nutrition_Item", ingredient_nutrients.Nutrition_Item_Id);
            return View(ingredient_nutrients);
        }

        // POST: Ingredient_Nutrients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromForm] Ingredient_Nutrients ingredient_Nutrients)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ingredient_Nutrients).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { IngredID = ingredient_Nutrients.Ingred_Id });
            }
            ViewBag.Ingred_Id = new SelectList(db.Ingredient.OrderBy(m => m.Ingred_name), "Ingredient_Id", "Ingred_name", ingredient_Nutrients.Ingred_Id);
            ViewBag.Nutrition_Item_Id = new SelectList(db.Nutrition.OrderBy(m => m.Nutrition_Item), "Nutrition_Item_Id", "Nutrition_Item", ingredient_Nutrients.Nutrition_Item_Id);
            return View(ingredient_Nutrients);
        }
        [HttpGet]
        public JsonResult GetData(int ingredId)
        {

            //BUILD VIEW FOR ANGULARJS RENDERING
            var query =
               from t1 in db.Ingredient_Nutrients.AsQueryable()
               join t2 in db.Ingredient.AsQueryable() on t1.Ingred_Id equals t2.Ingredient_Id into t2g
               from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t3 in db.Nutrition.AsQueryable() on t1.Nutrition_Item_Id equals t3.Nutrition_Item_Id
               where t1.Ingred_Id == ingredId
               orderby t3.Nutrition_Item
               select new View_Angular_Ingredient_Nutrients_Details()
               {
                   Id = t1.Id,
                   Customer_Id = t1.Customer_Id,
                   Ingred_Id = t1.Ingred_Id,
                   Ingred_name = t2.Ingred_name,
                   Nutrition_Item_Id = t1.Nutrition_Item_Id,
                   Nutrition_Item = t3.Nutrition_Item,
                   Nut_per_100_grams = t1.Nut_per_100_grams,
                   OrderOnNutrientPanel = t3.OrderOnNutrientPanel,
                   ShowOnNutrientPanel = t3.ShowOnNutrientPanel,

               };


            List<View_Angular_Ingredient_Nutrients_Details> ingredientcomponent = query.ToList();

            if (ingredientcomponent != null)
            {
                return Json(ingredientcomponent);
            }
            return Json(new { Status = "Failure" });
        }
        //UPDATE FROM ANGULAR

        [HttpPost]
        public JsonResult UpdateFromAngularController([FromBody]IEnumerable<View_Angular_Ingredient_Nutrients_Details> jsonstring)
        {

            foreach (View_Angular_Ingredient_Nutrients_Details thing in jsonstring)
            {
                if (thing.Nut_per_100_grams > 0)
                {

                    //DIRECT LINQ TO DB REPLACING STORED PROCEDURE
                    Ingredient_Nutrients ingredient_nutrients = db.Ingredient_Nutrients.Where(i => i.Id == thing.Id).Single();

                    ingredient_nutrients.Nut_per_100_grams = thing.Nut_per_100_grams;

                    db.SaveChanges();

                }

            }


            return Json(jsonstring);

        }
        public class GetAll
        {
            public string FirstName { get; set; }
            public string SecondName { get; set; }
            public int Ingred_Id { get; set; }
            public int Nutrition_Item_Id { get; set; }

        }

        // GET: Ingredient_Nutrients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }

            var Ingredients_Nutrients = db.Ingredient_Nutrients.Where(m => m.Id == id).Include(r => r.Ingredients).Include(r => r.Nutrition);

            // var recipe_Ingredients = recipe_Ingredients_Temp.Where(m => m.Id == id);

            if (Ingredients_Nutrients == null)
            {
                return NotFound();
            }
            //GOT TO BE A BETTER WAY THAN BUILD THE MODEL BY HAND:/
            Ingredient_Nutrients rIngreds = new Ingredient_Nutrients();
            Ingredient tIngred = new Ingredient();
            Nutrition tNut = new Nutrition();

            foreach (Ingredient_Nutrients thing in Ingredients_Nutrients)
            {
                tIngred = thing.Ingredients;
                tNut = thing.Nutrition;

                rIngreds.Id = thing.Id;
                rIngreds.Ingred_Id = thing.Ingred_Id;
                rIngreds.Ingredients = tIngred;
                rIngreds.Nutrition_Item_Id = thing.Nutrition_Item_Id;
                rIngreds.Nutrition = tNut;

            }

            //Ingredient_Nutrients ingredient_Nutrients = db.Ingredient_Nutrients.Find(id);
            //if (ingredient_Nutrients == null)
            //{
            //    return NotFound();
            //}

            return View(rIngreds);
        }

        // POST: Ingredient_Nutrients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ingredient_Nutrients ingredient_Nutrients = db.Ingredient_Nutrients.Find(id);


            try
            {
                db.Ingredient_Nutrients.Remove(ingredient_Nutrients);
                db.SaveChanges();
                return RedirectToAction("Index", new { IngredID = ingredient_Nutrients.Ingred_Id });
            }
           
            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(ingredient_Nutrients);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "There was an issue deleting this Nutrient from this Ingredient.";
                return View(ingredient_Nutrients);
            }




        }
        public ActionResult PrimeIngredNutrient(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }


            Ingredient ingredient = db.Ingredient.Find(id);

            if (ingredient == null)
            {
                return NotFound();
            }
            return View(ingredient);
        }
        [HttpPost, ActionName("PrimeIngredNutrient")]
        [ValidateAntiForgeryToken]
        public ActionResult PrimeIngredNutrientConfirmed(int id)
        {

            Ingredient ingredient = db.Ingredient.Find(id);
            //NEED TO ADD GROUP ID LATER
            var nutrients = db.Nutrition.Where(m => m.ShowOnNutrientPanel == true).ToList();

            foreach (Nutrition thing in nutrients)
            {
                Ingredient_Nutrients ingredient_nutrients = new Ingredient_Nutrients() {
                    Ingred_Id = ingredient.Ingredient_Id,
                    Nutrition_Item_Id = thing.Nutrition_Item_Id,
                    Nut_per_100_grams = 0               
                
                };
                //UPDATE DB - INSERT
                db.Ingredient_Nutrients.Add(ingredient_nutrients);
                db.SaveChanges();
                
            }

           
            return RedirectToAction("Index", new { IngredID = ingredient.Ingredient_Id });



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
