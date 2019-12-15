using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recitopia.Data;
using Recitopia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recitopia.Controllers
{
    public class Recipe_IngredientsController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public Recipe_IngredientsController(RecitopiaDBContext recitopiaDbContext)
        {
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
        }

        [Authorize]
        public async Task<ActionResult> Index(int recipeId)
        {
            var customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));

            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            var recipe = await _recitopiaDbContext.Recipe.FindAsync(recipeId);

            if (recipe == null)
            {
                return NotFound();
            }

            var recipeIngredients = await _recitopiaDbContext.Recipe_Ingredients
                .Include(ri => ri.Ingredient)
                .Where(ri => ri.Customer_Id == customerId && ri.Recipe_Id == recipeId)
                .OrderBy(ri => ri.Ingredient.Ingred_name)
                .ToListAsync();

            ViewBag.RecipeName = recipe.Recipe_Name;
            ViewBag.RecipeNameID = recipeId;
            ViewBag.GramWieght = recipeIngredients.Sum(ri => ri.Amount_g);

            return View(recipeIngredients);
        }

        [HttpGet]
        public async Task<JsonResult> GetData(int recipeId)
        {
            var customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));

            var recipeIngredientDetails = await _recitopiaDbContext.Recipe_Ingredients
                .Include(ri => ri.Ingredient)
                .Include(ri => ri.Recipe)
                .Where(ri => ri.Customer_Id == customerId && ri.Recipe_Id == recipeId)
                .Select(ri => new View_Angular_Recipe_Ingredients_Details()
                {
                    Id = ri.Id,
                    Customer_Id = ri.Customer_Id,
                    Ingredient_Id = ri.Ingredient_Id,
                    Ingred_name =  ri.Ingredient.Ingred_name,
                    Recipe_Id = ri.Recipe_Id,
                    Recipe_Name = ri.Recipe.Recipe_Name,
                    Amount_g = ri.Amount_g
                })
                .ToListAsync();

            return recipeIngredientDetails != null 
                ? Json(recipeIngredientDetails) 
                : Json(new { Status = "Failure" });
        }

        public async Task<ActionResult> Details(int id)
        {
            var recipeIngredients = await _recitopiaDbContext.Recipe_Ingredients.FindAsync(id);

            if (recipeIngredients == null)
            {
                return NotFound();
            }

            return View(recipeIngredients);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateFromAngularController([FromBody]List<View_Angular_Recipe_Ingredients_Details> recipeIngredientsDetails)
        {
            var recipeId = recipeIngredientsDetails.First().Recipe_Id;

            foreach (View_Angular_Recipe_Ingredients_Details detail in recipeIngredientsDetails)
            {
                // Dave: why only update if the weight is greater than zero?
                if (detail.Amount_g > 0)
                {
                    var recipeIngredient = await _recitopiaDbContext.Recipe_Ingredients
                        .SingleAsync(i => i.Id == detail.Id);

                    recipeIngredient.Amount_g = detail.Amount_g;

                    await _recitopiaDbContext.SaveChangesAsync();
                }
            }

            var recipe = await _recitopiaDbContext.Recipe.SingleAsync(i => i.Recipe_Id == recipeId);
            recipe.LastModified = DateTime.UtcNow;

            await _recitopiaDbContext.SaveChangesAsync();

            return Json(recipeIngredientsDetails);
        }

        // GET: Recipe_Ingredients/Create
        public ActionResult Create(int recipeID)
        {
            //get recipe name from db and pass in viewbag
            Recipe recipe = _recitopiaDbContext.Recipe.Find(recipeID);

            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            //Assign to temp local to put on view
            ViewBag.RecipeName = recipe.Recipe_Name;
            ViewBag.Rec_Id = recipeID;

            ViewBag.Ingredient_Id = new SelectList(_recitopiaDbContext.Ingredient.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Ingred_name), "Ingredient_Id", "Ingred_name");
            ViewBag.Recipe_Id = new SelectList(_recitopiaDbContext.Recipe.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Recipe_Name), "Recipe_Id", "Recipe_Name");

            //CREATE LIST OF PREVIOUS ADDED INGREDIENTS
            var queryIL =
               from t1 in _recitopiaDbContext.Recipe_Ingredients.AsQueryable()
               join t2 in _recitopiaDbContext.Recipe.AsQueryable() on t1.Recipe_Id equals t2.Recipe_Id into t2g
               from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t3 in _recitopiaDbContext.Ingredient.AsQueryable() on t1.Ingredient_Id equals t3.Ingredient_Id
               where t1.Recipe_Id == recipeID
               where t1.Customer_Id == CustomerId
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

            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            if (RID > 0)
            {

                recipe_Ingredients.Recipe_Id = RID;
                recipe_Ingredients.Customer_Id = CustomerId;
                _recitopiaDbContext.Recipe_Ingredients.Add(recipe_Ingredients);
                _recitopiaDbContext.SaveChanges();

                //DIRECT LINQ TO DB REPLACING STORED PROCEDURE
                Recipe recipeFind = _recitopiaDbContext.Recipe.Where(i => i.Recipe_Id == recipe_Ingredients.Recipe_Id).Single();

                recipeFind.LastModified = DateTime.UtcNow;

                _recitopiaDbContext.SaveChanges();

                //CREATE LIST OF PREVIOUS ADDED INGREDIENTS
                var queryIL =
                   from t1 in _recitopiaDbContext.Recipe_Ingredients.AsQueryable()
                   join t2 in _recitopiaDbContext.Recipe.AsQueryable() on t1.Recipe_Id equals t2.Recipe_Id into t2g
                   from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
                   join t3 in _recitopiaDbContext.Ingredient.AsQueryable() on t1.Ingredient_Id equals t3.Ingredient_Id
                   where t1.Recipe_Id == RID
                   where t1.Customer_Id == CustomerId
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

            ViewBag.Ingredient_Id = new SelectList(_recitopiaDbContext.Ingredient.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Ingred_name), "Ingredient_Id", "Ingred_name", recipe_Ingredients.Ingredient_Id);
            ViewBag.Recipe_Id = new SelectList(_recitopiaDbContext.Recipe.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Recipe_Name), "Recipe_Id", "Recipe_Name", recipe_Ingredients.Recipe_Id);




            return View(recipe_Ingredients);
        }

        // GET: Recipe_Ingredients/Edit/5
        public ActionResult Edit(int? id)
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

            Recipe_Ingredients recipe_Ingredients = _recitopiaDbContext.Recipe_Ingredients.Find(id);
            if (recipe_Ingredients == null)
            {
                return NotFound();
            }

            //get recipe name from db and pass in viewbag
            Recipe recipe = _recitopiaDbContext.Recipe.Find(recipe_Ingredients.Recipe_Id);

            //Assign to temp local to put on view
            ViewBag.RecipeName = recipe.Recipe_Name;
            ViewBag.Rec_Id = recipe_Ingredients.Recipe_Id;


            ViewBag.Ingredient_Id = new SelectList(_recitopiaDbContext.Ingredient.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Ingred_name), "Ingredient_Id", "Ingred_name", recipe_Ingredients.Ingredient_Id);
            ViewBag.Recipe_Id = new SelectList(_recitopiaDbContext.Recipe.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Recipe_Name), "Recipe_Id", "Recipe_Name", recipe_Ingredients.Recipe_Id);
            return View(recipe_Ingredients);
        }

        // POST: Recipe_Ingredients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromForm] Recipe_Ingredients recipe_Ingredients)
        {

            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (ModelState.IsValid)
            {
                recipe_Ingredients.Customer_Id = CustomerId;
                _recitopiaDbContext.Entry(recipe_Ingredients).State = EntityState.Modified;
                _recitopiaDbContext.SaveChanges();

                //DIRECT LINQ TO DB REPLACING STORED PROCEDURE
                Recipe recipeFind = _recitopiaDbContext.Recipe.Where(i => i.Recipe_Id == recipe_Ingredients.Recipe_Id).Single();

                recipeFind.LastModified = DateTime.UtcNow;

                _recitopiaDbContext.SaveChanges();


                return RedirectToAction("Index", new { recipeID = recipe_Ingredients.Recipe_Id });
            }
            ViewBag.Ingredient_Id = new SelectList(_recitopiaDbContext.Ingredient.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Ingred_name), "Ingredient_Id", "Ingred_name", recipe_Ingredients.Ingredient_Id);
            ViewBag.Recipe_Id = new SelectList(_recitopiaDbContext.Recipe.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Recipe_Name), "Recipe_Id", "Recipe_Name", recipe_Ingredients.Recipe_Id);
            return View(recipe_Ingredients);
        }

        // GET: Recipe_Ingredients/Delete/5
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
            var recipe_Ingredients = _recitopiaDbContext.Recipe_Ingredients.Where(m => m.Id == id).Include(r => r.Ingredient).Where(m => m.Customer_Id == CustomerId).Include(r => r.Recipe).Where(m => m.Customer_Id == CustomerId);

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
                rIngreds.Customer_Id = thing.Customer_Id;
            }
            


            return View(rIngreds);
        }

        // POST: Recipe_Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Recipe_Ingredients recipe_Ingredients = _recitopiaDbContext.Recipe_Ingredients.Find(id);

            try
            {
                _recitopiaDbContext.Recipe_Ingredients.Remove(recipe_Ingredients);
                _recitopiaDbContext.SaveChanges();
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
    }
}
