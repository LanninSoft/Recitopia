using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
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
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if  (customerGuid == null || customerGuid.Trim() == "")
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
                .Where(ri => ri.Customer_Guid == customerGuid && ri.Recipe_Id == recipeId)
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
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            var recipeIngredientDetails = await _recitopiaDbContext.Recipe_Ingredients
                .Include(ri => ri.Ingredient)
                .Include(ri => ri.Recipe)
                .Where(ri => ri.Customer_Guid == customerGuid && ri.Recipe_Id == recipeId)
                .Select(ri => new View_Angular_Recipe_Ingredients_Details()
                {
                    Id = ri.Id,
                    Customer_Guid = ri.Customer_Guid,
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
        [HttpGet]
        public async Task<JsonResult> GetDataCreate(int recipe_Id)
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");            

            var usedIngredientsForRecipe = await _recitopiaDbContext.Recipe_Ingredients
                .Where(m => m.Customer_Guid == customerGuid && m.Recipe_Id == recipe_Id)
                .Select(ri => new View_Angular_Recipe_Ingredients_Details()
                { 
                    Ingredient_Id = ri.Ingredient_Id
                })
                .ToListAsync();

                     
            List<int> IngredientIdList = new List<int>();

            foreach (View_Angular_Recipe_Ingredients_Details thing in usedIngredientsForRecipe)
            {
                IngredientIdList.Add(thing.Ingredient_Id);
            }

            
            var filterIngredients = await _recitopiaDbContext.Ingredient
                .Include(f => f.Vendor)
                .Where(f => !IngredientIdList.Contains(f.Ingredient_Id) && f.Customer_Guid == customerGuid)
                .OrderBy(i => i.Ingred_name)
                .Select(ri => new View_Angular_Ingredients_Details()
                {
                    Ingredient_Id = ri.Ingredient_Id,
                    Customer_Guid = ri.Customer_Guid,
                    Ingred_name = ri.Ingred_name,
                    Cost_per_lb = ri.Cost_per_lb,
                    Cost = ri.Cost,
                    Package = ri.Package,
                    Vendor_Name = ri.Vendor.Vendor_Name,
                    Amount_g = 0
                })
                .ToListAsync();


            return filterIngredients != null
                 ? Json(filterIngredients)
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
                
                var recipeIngredient = await _recitopiaDbContext.Recipe_Ingredients
                        .SingleAsync(i => i.Id == detail.Id);                

                if (detail.Amount_g >= 0)
                {
                    recipeIngredient.Amount_g = detail.Amount_g;
                    
                }
                else
                {
                    recipeIngredient.Amount_g = 0;
                    
                }
                await _recitopiaDbContext.SaveChangesAsync();
            }

            var recipe = await _recitopiaDbContext.Recipe.SingleAsync(i => i.Recipe_Id == recipeId);
            recipe.LastModified = DateTime.UtcNow;

            await _recitopiaDbContext.SaveChangesAsync();

            return Json(recipeIngredientsDetails);
        }

        // GET: Recipe_Ingredients/Create
       public async Task<ActionResult> Create(int recipeID)
        {
            //get recipe name from db and pass in viewbag
            Recipe recipe = await _recitopiaDbContext.Recipe.FindAsync(recipeID);

            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if  (customerGuid == null || customerGuid.Trim() == "")
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            //Assign to temp local to put on view
            ViewBag.RecipeName = recipe.Recipe_Name;
            ViewBag.Rec_Id = recipeID;

            ViewBag.Ingredient_Id = new SelectList(await _recitopiaDbContext.Ingredient.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Ingred_name).ToListAsync(), "Ingredient_Id", "Ingred_name");
            ViewBag.Recipe_Id = new SelectList(await _recitopiaDbContext.Recipe.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Recipe_Name).ToListAsync(), "Recipe_Id", "Recipe_Name");

            //CREATE LIST OF PREVIOUS ADDED INGREDIENTS
            var recipeIngredients = await _recitopiaDbContext.Recipe_Ingredients
                            .Include(ri => ri.Recipe)
                            .Include(ri => ri.Ingredient)
                            .Where(ri => ri.Customer_Guid == customerGuid && ri.Recipe_Id == recipeID)
                            .OrderBy(ri => ri.Ingredient.Ingred_name)
                            .Select(ri => new View_All_Recipe_Ingredients()
                            {
                                Id = ri.Id,
                                Customer_Guid = ri.Customer_Guid,
                                Recipe_Id = ri.Recipe_Id,
                                Ingredient_Id = ri.Ingredient_Id,
                                Amount_g = ri.Amount_g,
                                Ingred_name = ri.Ingredient.Ingred_name,
                                Ingred_Comp_name = ri.Ingredient.Ingred_Comp_name,
                                Cost_per_lb = ri.Ingredient.Cost_per_lb,
                                Cost = ri.Ingredient.Cost,
                                Package = ri.Ingredient.Package,
                                Recipe_Name = ri.Recipe.Recipe_Name
                            })
                            .ToListAsync();

            //build list to populate previously added items
            List<string> ingList = new List<string>();

            foreach (View_All_Recipe_Ingredients ingred in recipeIngredients)
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
       public async Task<ActionResult> Create([FromForm] Recipe_Ingredients recipe_Ingredients, int RID)
        {

            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");
            if  (customerGuid == null || customerGuid.Trim() == "")
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            if (RID > 0)
            {

                recipe_Ingredients.Recipe_Id = RID;
                recipe_Ingredients.Customer_Guid = customerGuid;
                await _recitopiaDbContext.Recipe_Ingredients.AddAsync(recipe_Ingredients);
                await _recitopiaDbContext.SaveChangesAsync();

                //DIRECT LINQ TO DB REPLACING STORED PROCEDURE
                Recipe recipeFind = await _recitopiaDbContext.Recipe.SingleAsync(i => i.Recipe_Id == recipe_Ingredients.Recipe_Id);

                recipeFind.LastModified = DateTime.UtcNow;

                await _recitopiaDbContext.SaveChangesAsync();

                //CREATE LIST OF PREVIOUS ADDED INGREDIENTS
                var recipeIngredients = await _recitopiaDbContext.Recipe_Ingredients
                            .Include(ri => ri.Recipe)
                            .Include(ri => ri.Ingredient)
                            .Where(ri => ri.Customer_Guid == customerGuid && ri.Recipe_Id == RID)
                            .OrderBy(ri => ri.Ingredient.Ingred_name)
                            .Select(ri => new View_All_Recipe_Ingredients()
                            {
                                Id = ri.Id,
                                Customer_Guid = ri.Customer_Guid,
                                Recipe_Id = ri.Recipe_Id,
                                Ingredient_Id = ri.Ingredient_Id,
                                Amount_g = ri.Amount_g,
                                Ingred_name = ri.Ingredient.Ingred_name,
                                Ingred_Comp_name = ri.Ingredient.Ingred_Comp_name,
                                Cost_per_lb = ri.Ingredient.Cost_per_lb,
                                Cost = ri.Ingredient.Cost,
                                Package = ri.Ingredient.Package,
                                Recipe_Name = ri.Recipe.Recipe_Name
                            })
                            .ToListAsync();


                //build list to populate previously added items
                List<string> ingList = new List<string>();

                foreach (View_All_Recipe_Ingredients ingred in recipeIngredients)
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

            ViewBag.Ingredient_Id = new SelectList(await _recitopiaDbContext.Ingredient.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Ingred_name).ToListAsync(), "Ingredient_Id", "Ingred_name", recipe_Ingredients.Ingredient_Id);
            ViewBag.Recipe_Id = new SelectList(await _recitopiaDbContext.Recipe.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Recipe_Name).ToListAsync(), "Recipe_Id", "Recipe_Name", recipe_Ingredients.Recipe_Id);




            return View(recipe_Ingredients);
        }
        [HttpPost]
        public async Task<JsonResult> CreateRecipeIngredient([FromBody]List<View_Angular_Ingredients_Details> recipeIngredientsDetails)
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");
            var recipeID = 0;

            foreach (View_Angular_Ingredients_Details detail in recipeIngredientsDetails)
            {
                recipeID = detail.recipe_Id;

                if (detail.Amount_g > 0)
                {
                    var recipeIngredient = new Recipe_Ingredients();

                    recipeIngredient.Amount_g = detail.Amount_g;

                    recipeIngredient.Recipe_Id = detail.recipe_Id;
                    recipeIngredient.Ingredient_Id = detail.Ingredient_Id;
                    recipeIngredient.Customer_Guid = customerGuid;

                    //Lets see if it's already in the db
                    var checkRecipeIngredients = await _recitopiaDbContext.Recipe_Ingredients
                        .Where(m => m.Customer_Guid == customerGuid && m.Ingredient_Id == detail.Ingredient_Id && m.Recipe_Id == detail.recipe_Id)
                        .FirstOrDefaultAsync();

                    if (checkRecipeIngredients == null)
                    {
                        await _recitopiaDbContext.Recipe_Ingredients.AddAsync(recipeIngredient);
                        await _recitopiaDbContext.SaveChangesAsync();
                    }

                    
                }
                
                
            }

            
            //return Json("Success");
            return Json(recipeID);
            //return RedirectToAction("Index", new { recipeId = recipeID });

        }

        // GET: Recipe_Ingredients/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }

             var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");
            if  (customerGuid == null || customerGuid.Trim() == "")
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            Recipe_Ingredients recipe_Ingredients = await _recitopiaDbContext.Recipe_Ingredients.FindAsync(id);
            if (recipe_Ingredients == null)
            {
                return NotFound();
            }

            //get recipe name from db and pass in viewbag
            Recipe recipe = await _recitopiaDbContext.Recipe.FindAsync(recipe_Ingredients.Recipe_Id);

            //Assign to temp local to put on view
            ViewBag.RecipeName = recipe.Recipe_Name;
            ViewBag.Rec_Id = recipe_Ingredients.Recipe_Id;


            ViewBag.Ingredient_Id = new SelectList(await _recitopiaDbContext.Ingredient.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Ingred_name).ToListAsync(), "Ingredient_Id", "Ingred_name", recipe_Ingredients.Ingredient_Id);
            ViewBag.Recipe_Id = new SelectList(await _recitopiaDbContext.Recipe.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Recipe_Name).ToListAsync(), "Recipe_Id", "Recipe_Name", recipe_Ingredients.Recipe_Id);
            return View(recipe_Ingredients);
        }

        // POST: Recipe_Ingredients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       public async Task<ActionResult> Edit([FromForm] Recipe_Ingredients recipe_Ingredients)
        {

            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");
            if (ModelState.IsValid)
            {
                recipe_Ingredients.Customer_Guid = customerGuid;

                _recitopiaDbContext.Entry(recipe_Ingredients).State = EntityState.Modified;

                await _recitopiaDbContext.SaveChangesAsync();

                //DIRECT LINQ TO DB REPLACING STORED PROCEDURE
                Recipe recipeFind = await _recitopiaDbContext.Recipe.SingleAsync(i => i.Recipe_Id == recipe_Ingredients.Recipe_Id);

                recipeFind.LastModified = DateTime.UtcNow;

                await _recitopiaDbContext.SaveChangesAsync();


                return RedirectToAction("Index", new { recipeID = recipe_Ingredients.Recipe_Id });
            }
            ViewBag.Ingredient_Id = new SelectList(await _recitopiaDbContext.Ingredient.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Ingred_name).ToListAsync(), "Ingredient_Id", "Ingred_name", recipe_Ingredients.Ingredient_Id);
            ViewBag.Recipe_Id = new SelectList(await _recitopiaDbContext.Recipe.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Recipe_Name).ToListAsync(), "Recipe_Id", "Recipe_Name", recipe_Ingredients.Recipe_Id);
            return View(recipe_Ingredients);
        }

        // GET: Recipe_Ingredients/Delete/5
       public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }

            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if  (customerGuid == null || customerGuid.Trim() == "")
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            var recipe_Ingredients = await _recitopiaDbContext.Recipe_Ingredients
                .Include(r => r.Ingredient)
                .Include(r => r.Recipe)
                .Where(m => m.Id == id).ToListAsync();

          
            if (recipe_Ingredients == null)
            {
                return NotFound();
            }
            //GOT TO BE A BETTER WAY THAN BUILD THE MODEL BY HAND:/
            Recipe_Ingredients rIngreds = new Recipe_Ingredients();
            Ingredient tIngred = new Ingredient();
            Recipe tRec = new Recipe();

            foreach (Recipe_Ingredients recipeIngredient in recipe_Ingredients)
            {
                tIngred = recipeIngredient.Ingredient;
                tRec = recipeIngredient.Recipe;
                
                rIngreds.Id = recipeIngredient.Id;
                rIngreds.Ingredient_Id = recipeIngredient.Ingredient_Id;
                rIngreds.Ingredient = tIngred;
                rIngreds.Recipe_Id = recipeIngredient.Recipe_Id;
                rIngreds.Recipe = tRec;
                rIngreds.Customer_Guid = recipeIngredient.Customer_Guid;
            }

            return View(rIngreds);
        }

        // POST: Recipe_Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
       public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Recipe_Ingredients recipe_Ingredients = await _recitopiaDbContext.Recipe_Ingredients.FindAsync(id);

            try
            {
                _recitopiaDbContext.Recipe_Ingredients.Remove(recipe_Ingredients);
                await _recitopiaDbContext.SaveChangesAsync();
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
