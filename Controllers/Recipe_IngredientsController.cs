using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Recitopia_LastChance.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace Recitopia_LastChance.Controllers
{
    public class Recipe_IngredientsController : AuthorizeController
    {
        private RecitopiaDBContext db = new RecitopiaDBContext();
        [Authorize]
        // GET: Recipe_Ingredients
        public ActionResult Index(int recipeID)
        {

            var recipe_Ingredients_Temp = db.Recipe_Ingredients.Include(r => r.Ingredient).Include(r => r.Recipe);
            //Filter and sort down to recipe id looking for
            var recipe_Ingredients = recipe_Ingredients_Temp.Where(i => i.Recipe_Id.Equals(recipeID)).OrderBy(i => i.Ingredient.Ingred_name);

            //get recipe name from db and pass in viewbag
            Recipe recipe = db.Recipe.Find(recipeID);

            //Assign to temp local to put on view
            ViewBag.RecipeName = recipe.Recipe_Name;



            decimal sumWieght = 0;
            //sum gram weight
            try
            {
                sumWieght = recipe_Ingredients.Sum(m => m.Amount_g);
            }
            catch (Exception)
            {
                sumWieght = 0;
            }

            ViewBag.RecipeNameID = recipeID;

            ViewBag.GramWieght = sumWieght;

            return View(recipe_Ingredients.ToList());
        }
        [HttpGet]
        public JsonResult GetData(int recipe_Id)
        {

            //BUILD VIEW FOR ANGULARJS RENDERING
            var query =
               from t1 in db.Recipe_Ingredients.AsQueryable()
               join t2 in db.Recipe.AsQueryable() on t1.Recipe_Id equals t2.Recipe_Id into t2g
               from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t3 in db.Ingredient.AsQueryable() on t1.Ingredient_Id equals t3.Ingredient_Id
               where t1.Recipe_Id == recipe_Id
               orderby t3.Ingred_name
               select new View_Angular_Recipe_Ingredients_Details()
               {
                   Id = t1.Id,
                   Customer_Id = t1.Customer_Id,
                   Ingredient_Id = t1.Ingredient_Id,
                   Ingred_name = t3.Ingred_name,
                   Recipe_Id = t1.Recipe_Id,
                   Recipe_Name = t2.Recipe_Name,
                   Amount_g = t1.Amount_g,
                  
               };

            List<View_Angular_Recipe_Ingredients_Details> ingredients = query.ToList();

            if (ingredients != null)
            {
                return Json(ingredients);
            }
            return Json(new { Status = "Failure" });
        }
        // GET: Recipe_Ingredients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Recipe_Ingredients recipe_Ingredients = db.Recipe_Ingredients.Find(id);
            if (recipe_Ingredients == null)
            {
                return NotFound();
            }
            return View(recipe_Ingredients);
        }

        //UPDATE FROM ANGULAR

        [HttpPost]
        public JsonResult UpdateFromAngularController([FromBody]IEnumerable<View_Angular_Recipe_Ingredients_Details> jsonstring)
        {

            var recipeId = 0;
            foreach (View_Angular_Recipe_Ingredients_Details thing in jsonstring)
            {
                recipeId = thing.Recipe_Id;

                if (thing.Amount_g > 0)
                {
                    //string str = Math.Round(thing.Amount_g, 3).ToString("0.000");

                    //DIRECT LINQ TO DB REPLACING STORED PROCEDURE
                    Recipe_Ingredients recipe_ingredient = db.Recipe_Ingredients.Where(i => i.Id == thing.Id).Single();

                    recipe_ingredient.Amount_g = thing.Amount_g;

                    db.SaveChanges();

                }

            }

            //DIRECT LINQ TO DB REPLACING STORED PROCEDURE
            Recipe recipeFind = db.Recipe.Where(i => i.Recipe_Id == recipeId).Single();

            recipeFind.LastModified = DateTime.Now;

            db.SaveChanges();

            return Json(jsonstring);

        }

        // GET: Recipe_Ingredients/Create
        public ActionResult Create(int recipeID)
        {
            //get recipe name from db and pass in viewbag
            Recipe recipe = db.Recipe.Find(recipeID);

            //Assign to temp local to put on view
            ViewBag.RecipeName = recipe.Recipe_Name;
            ViewBag.Rec_Id = recipeID;

            ViewBag.Ingredient_Id = new SelectList(db.Ingredient.OrderBy(m => m.Ingred_name), "Ingredient_Id", "Ingred_name");
            ViewBag.Recipe_Id = new SelectList(db.Recipe.OrderBy(m => m.Recipe_Name), "Recipe_Id", "Recipe_Name");

            //CREATE LIST OF PREVIOUS ADDED INGREDIENTS
            var queryIL =
               from t1 in db.Recipe_Ingredients.AsQueryable()
               join t2 in db.Recipe.AsQueryable() on t1.Recipe_Id equals t2.Recipe_Id into t2g
               from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t3 in db.Ingredient.AsQueryable() on t1.Ingredient_Id equals t3.Ingredient_Id
               where t1.Recipe_Id == recipeID
               orderby t3.Ingred_name
               select new View_All_Recipe_Ingredients()
               {
                   Id = t1.Id,
                   Customer_Id = t1.Customer_Id,
                   Recipe_Id = t1.Recipe_Id,
                   Ingredient_Id = t1.Ingredient_Id,
                   Amount_g = t1.Amount_g,
                   Ingred_name = t3.Ingred_name,
                   Ingred_Comp_name = t3.Ingred_Comp_name,
                   Cost_per_lb = t3.Cost_per_lb,
                   Cost = t3.Cost,
                   Package = t3.Package,
                   Recipe_Name = t2.Recipe_Name
               };
            //Get View and filter and sort            
            var TempV = queryIL.ToList();

            //build list to populate previously added items
            List<string> ingList = new List<string>();

            foreach (View_All_Recipe_Ingredients ingred in TempV)
            {
                if (ingred.Amount_g.ToString() != null)
                {
                    ingList.Add(ingred.Ingred_name + "/" + ingred.Amount_g.ToString() + "g");
                }
                else
                {
                    ingList.Add(ingred.Ingred_name + "/" + 0.ToString() + "g");
                }
            }

            ViewBag.Ingredients = ingList;

            return View();
        }

        // POST: Recipe_Ingredients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] Recipe_Ingredients recipe_Ingredients, int RID)
        {
            if (RID > 0)
            {

                recipe_Ingredients.Recipe_Id = RID;
                db.Recipe_Ingredients.Add(recipe_Ingredients);
                db.SaveChanges();

                //DIRECT LINQ TO DB REPLACING STORED PROCEDURE
                Recipe recipeFind = db.Recipe.Where(i => i.Recipe_Id == recipe_Ingredients.Recipe_Id).Single();

                recipeFind.LastModified = DateTime.Now;

                db.SaveChanges();

                //CREATE LIST OF PREVIOUS ADDED INGREDIENTS
                var queryIL =
                   from t1 in db.Recipe_Ingredients.AsQueryable()
                   join t2 in db.Recipe.AsQueryable() on t1.Recipe_Id equals t2.Recipe_Id into t2g
                   from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
                   join t3 in db.Ingredient.AsQueryable() on t1.Ingredient_Id equals t3.Ingredient_Id
                   where t1.Recipe_Id == RID
                   orderby t3.Ingred_name
                   select new View_All_Recipe_Ingredients()
                   {
                       Id = t1.Id,
                       Customer_Id = t1.Customer_Id,
                       Recipe_Id = t1.Recipe_Id,
                       Ingredient_Id = t1.Ingredient_Id,
                       Amount_g = t1.Amount_g,
                       Ingred_name = t3.Ingred_name,
                       Ingred_Comp_name = t3.Ingred_Comp_name,
                       Cost_per_lb = t3.Cost_per_lb,
                       Cost = t3.Cost,
                       Package = t3.Package,
                       Recipe_Name = t2.Recipe_Name
                   };
                var TempV = queryIL.ToList();

                //build list to populate previously added items
                List<string> ingList = new List<string>();

                foreach (View_All_Recipe_Ingredients ingred in TempV)
                {
                    if (ingred.Amount_g.ToString() != null)
                    {
                        ingList.Add(ingred.Ingred_name + "/" + ingred.Amount_g.ToString() + "g");
                    }
                    else
                    {
                        ingList.Add(ingred.Ingred_name + "/" + 0.ToString() + "g");
                    }

                }

                ViewBag.Ingredients = ingList;



                return RedirectToAction("Index", new { recipeID = recipe_Ingredients.Recipe_Id });
            }

            ViewBag.Ingredient_Id = new SelectList(db.Ingredient.OrderBy(m => m.Ingred_name), "Ingredient_Id", "Ingred_name", recipe_Ingredients.Ingredient_Id);
            ViewBag.Recipe_Id = new SelectList(db.Recipe.OrderBy(m => m.Recipe_Name), "Recipe_Id", "Recipe_Name", recipe_Ingredients.Recipe_Id);




            return View(recipe_Ingredients);
        }

        // GET: Recipe_Ingredients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Recipe_Ingredients recipe_Ingredients = db.Recipe_Ingredients.Find(id);
            if (recipe_Ingredients == null)
            {
                return NotFound();
            }

            //get recipe name from db and pass in viewbag
            Recipe recipe = db.Recipe.Find(recipe_Ingredients.Recipe_Id);

            //Assign to temp local to put on view
            ViewBag.RecipeName = recipe.Recipe_Name;
            ViewBag.Rec_Id = recipe_Ingredients.Recipe_Id;


            ViewBag.Ingredient_Id = new SelectList(db.Ingredient.OrderBy(m => m.Ingred_name), "Ingredient_Id", "Ingred_name", recipe_Ingredients.Ingredient_Id);
            ViewBag.Recipe_Id = new SelectList(db.Recipe.OrderBy(m => m.Recipe_Name), "Recipe_Id", "Recipe_Name", recipe_Ingredients.Recipe_Id);
            return View(recipe_Ingredients);
        }

        // POST: Recipe_Ingredients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromForm] Recipe_Ingredients recipe_Ingredients)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipe_Ingredients).State = EntityState.Modified;
                db.SaveChanges();

                //DIRECT LINQ TO DB REPLACING STORED PROCEDURE
                Recipe recipeFind = db.Recipe.Where(i => i.Recipe_Id == recipe_Ingredients.Recipe_Id).Single();

                recipeFind.LastModified = DateTime.Now;

                db.SaveChanges();


                return RedirectToAction("Index", new { recipeID = recipe_Ingredients.Recipe_Id });
            }
            ViewBag.Ingredient_Id = new SelectList(db.Ingredient.OrderBy(m => m.Ingred_name), "Ingredient_Id", "Ingred_name", recipe_Ingredients.Ingredient_Id);
            ViewBag.Recipe_Id = new SelectList(db.Recipe.OrderBy(m => m.Recipe_Name), "Recipe_Id", "Recipe_Name", recipe_Ingredients.Recipe_Id);
            return View(recipe_Ingredients);
        }

        // GET: Recipe_Ingredients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }

            var recipe_Ingredients = db.Recipe_Ingredients.Where(m => m.Id == id).Include(r => r.Ingredient).Include(r => r.Recipe);

           // var recipe_Ingredients = recipe_Ingredients_Temp.Where(m => m.Id == id);

            if (recipe_Ingredients == null)
            {
                return NotFound();
            }
            //GOT TO BE A BETTER WAY THAN BUILD THE MODEL BY HAND:/
            Recipe_Ingredients rIngreds = new Recipe_Ingredients();
            Ingredient tIngred = new Ingredient();
            Recipe tRec = new Recipe();

            foreach (Recipe_Ingredients thing in recipe_Ingredients)
            {
                tIngred = thing.Ingredient;
                tRec = thing.Recipe;
                
                rIngreds.Id = thing.Id;
                rIngreds.Ingredient_Id = thing.Ingredient_Id;
                rIngreds.Ingredient = tIngred;
                rIngreds.Recipe_Id = thing.Recipe_Id;
                rIngreds.Recipe = tRec;

            }
            


            return View(rIngreds);
        }

        // POST: Recipe_Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Recipe_Ingredients recipe_Ingredients = db.Recipe_Ingredients.Find(id);

            try
            {
                db.Recipe_Ingredients.Remove(recipe_Ingredients);
                db.SaveChanges();
                return RedirectToAction("Index", new { recipeID = recipe_Ingredients.Recipe_Id });
            }
            
            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(recipe_Ingredients);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "This was a problem deleting this Ingredient from this Recipe.";
                return View(recipe_Ingredients);
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
