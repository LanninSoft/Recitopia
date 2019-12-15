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

    public class RecipesController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public RecipesController(RecitopiaDBContext recitopiaDbContext)
        {
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
        }

        [Authorize]
        public async Task<ActionResult> Index()
        {
            var customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));

            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            var recipes = await _recitopiaDbContext.Recipe
                .Include(r => r.Meal_Category)
                .Where(r => r.Customer_Id == customerId)
                .OrderBy(r => r.Recipe_Name)
                .ToListAsync();

            return recipes != null ? View(recipes) : View();
        }
        
        [HttpGet]
        public JsonResult GetData()
        {
            var customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            
            //BUILD VIEW FOR ANGULARJS RENDERING
            var query =
               from t1 in _recitopiaDbContext.Recipe.AsQueryable()
               join t2 in _recitopiaDbContext.Meal_Category.AsQueryable() on t1.Category_Id equals t2.Category_Id  into t2g
               from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t3 in _recitopiaDbContext.Serving_Sizes.AsQueryable() on t1.SS_Id equals t3.SS_Id
               where t1.Customer_Id == customerId
               orderby t1.Recipe_Name
               select new View_Angular_Recipe_Details()
               {
                   Recipe_Id = t1.Recipe_Id,
                   Customer_Id = t1.Customer_Id,
                   Recipe_Name = t1.Recipe_Name,
                   Category_Id = t1.Category_Id,
                   Category_Name = t2.Category_Name,
                   Gluten_Free = t1.Gluten_Free,
                   UPC = t1.UPC,
                   Notes = t1.Notes,
                   LaborCost = t1.LaborCost,
                   SS_Id = t1.SS_Id,
                   Serving_Size = t3.Serving_Size,
                   LastModified = t1.LastModified,
                   
               };

            List<View_Angular_Recipe_Details> recipe = query.ToList();

            if (recipe != null)
            {
                return Json(recipe);
            }

            return Json(new { Status = "Failure" });
        }

        public async Task<ActionResult> Details(int id)
        {
            var recipe = await _recitopiaDbContext.Recipe.FindAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }

            var customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));

            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            //----------------------------------------------------
            //GET MAIN INGREDIENT LIST AND BUILD OUT COST COLUMN - 2 VIEWBAGS 
            //---------------------------------------------------

            //BUILD OUT RECIPE INGREDIENT ITEMS
            var query =
               from t1 in _recitopiaDbContext.Recipe_Ingredients.AsQueryable()
               join t2 in _recitopiaDbContext.Recipe.AsQueryable() on t1.Recipe_Id equals t2.Recipe_Id into t2g
               from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t3 in _recitopiaDbContext.Ingredient.AsQueryable() on t1.Ingredient_Id equals t3.Ingredient_Id
               where t1.Recipe_Id == id
               where t1.Customer_Id == customerId
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

          
            List<View_All_Recipe_Ingredients> recipe_ing2 = query.ToList();

            recipe_ing2 = recipe_ing2.OrderBy(m => m.Ingred_name).ToList();

            decimal netTotal = 0;
            decimal netGramTotal = 0;
            decimal netGramTotalP = 0;


            decimal grandTotalG = 0;
            decimal grandTotalC = 0;

            //USED TO CALCULATE
            decimal ingCost = 0;
            decimal amountG = 0;

            decimal costLb = 0;
            decimal gToLbConversion = (decimal)453.592;

            //REGULAR INGREDIENTS           
            List<IList<string>> cols2 = new List<IList<string>>();

            foreach (View_All_Recipe_Ingredients comp2 in recipe_ing2)
            {
                if (comp2.Package == false)
                {
                    //CALCULATE INGREDIENT COST
                    if (!comp2.Amount_g.Equals(null))
                    {
                        amountG = (decimal)comp2.Amount_g;

                    }
                    else
                    {
                        amountG = 0;

                    }
                    netGramTotal += amountG;


                    if (!comp2.Cost_per_lb.Equals(null))
                    {
                        costLb = (decimal)comp2.Cost_per_lb;
                    }
                    else
                    {
                        costLb = 0;
                    }

                    ingCost = (amountG / gToLbConversion) * costLb;

                    //ADD TO INGREDIENT SUBTOTAL
                    netTotal += ingCost;

                    cols2.Add(new List<string> { comp2.Ingred_name, amountG.ToString(), Math.Round(costLb, 3).ToString(), Math.Round(ingCost, 3).ToString() });

                }

            }
            //ADD TO VIEWBAG LISTS
            ViewBag.MIComponents = cols2;
            ViewBag.MIComponentsSubtotal = Math.Round(netTotal, 3);
            ViewBag.MICGramsSubtotal = Math.Round(netGramTotal, 3);

            //ADD TO GRAND TOTAL
            grandTotalG += Math.Round(netGramTotal, 3);
            grandTotalC += Math.Round(netTotal, 3);

            //PACKAGING
            //RESET
            netTotal = 0;

            List<IList<string>> cols3 = new List<IList<string>>();

            foreach (View_All_Recipe_Ingredients comp3 in recipe_ing2)
            {

                if (comp3.Package == true)
                {
                    //CALCULATE INGREDIENT COST
                    if (!comp3.Amount_g.Equals(null))
                    {
                        amountG = (decimal)comp3.Amount_g;
                    }
                    else
                    {
                        amountG = 0;
                    }
                    netGramTotalP += amountG;

                    if (!comp3.Cost.Equals(null))
                    {
                        costLb = (decimal)comp3.Cost;
                    }
                    else
                    {
                        costLb = 0;
                    }

                    //ingCost = (amountG / gToLbConversion) * costLb;

                    //ADD TO INGREDIENT SUBTOTAL
                    netTotal += costLb;

                    cols3.Add(new List<string> { comp3.Ingred_name, amountG.ToString(), Math.Round(costLb, 3).ToString() });
                }

            }

            ViewBag.MIPComponents = cols3;

            ViewBag.MIPComponentsSubtotal = Math.Round(netTotal, 3);

            ViewBag.MIPCGramsSubtotal = Math.Round(netGramTotal, 3);

            //ADD TO  TOTAL
            grandTotalG += Math.Round(netGramTotalP + netGramTotal, 3);
            grandTotalC += Math.Round(netTotal, 3);

            ViewBag.GramsGTotal = grandTotalG;
            ViewBag.CostGTotal = grandTotalC;

            ViewBag.CostGSubtotal = ViewBag.MIComponentsSubtotal + ViewBag.MIPComponentsSubtotal;
            ViewBag.GramSubtotalPackaging = Math.Round(netGramTotalP, 3);
            ViewBag.GramTotal = ViewBag.GramSubtotalPackaging + ViewBag.MICGramsSubtotal;

            //GRANDTOTAL = Labor and Ingredient
            decimal recipeLabor = 0;
            if (!recipe.LaborCost.Equals(null))
            {
                recipeLabor = (decimal)recipe.LaborCost + grandTotalC;
            }
            else
            {
                recipeLabor = grandTotalC;
            }
            ViewBag.CostGRANDTotal = recipeLabor;

            //----------------------------------------------------
            //GET NUTRITION 
            //---------------------------------------------------

            //HOLDS SUMMERIZED DATA
            var ListFields = new List<View_All_Ingredient_Nutrients>();

            //BUILD OUT NUTRITION PANEL
            var queryRI =
               from t1 in _recitopiaDbContext.Recipe_Ingredients.AsQueryable()
               join t2 in _recitopiaDbContext.Recipe.AsQueryable() on t1.Recipe_Id equals t2.Recipe_Id into t2g
               from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t4 in _recitopiaDbContext.Serving_Sizes.AsQueryable() on t2.SS_Id equals t4.SS_Id into t4g
               from t4 in t4g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t5 in _recitopiaDbContext.Ingredient_Nutrients.AsQueryable() on t1.Ingredient_Id equals t5.Ingred_Id into t5g
               from t5 in t5g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t3 in _recitopiaDbContext.Ingredient.AsQueryable() on t1.Ingredient_Id equals t3.Ingredient_Id into t3g
               from t3 in t3g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t6 in _recitopiaDbContext.Nutrition.AsQueryable() on t5.Nutrition_Item_Id equals t6.Nutrition_Item_Id into t6g
               from t6 in t6g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins

               where t1.Recipe_Id == id && t6.ShowOnNutrientPanel == true
               orderby t6.OrderOnNutrientPanel, t6.Nutrition_Item
               select new View_Nutrition_Panel()
               {
                   Customer_Id = t1.Customer_Id,
                   Recipe_Id = t1.Recipe_Id,
                   Recipe_Name = t2.Recipe_Name,
                   Serving_Size = t4.Serving_Size,
                   Ingred_Id = t1.Ingredient_Id,
                   Ingred_name = t3.Ingred_name,
                   ShowOnNutrientPanel = t6.ShowOnNutrientPanel,
                   OrderOnNutrientPanel = t6.OrderOnNutrientPanel,
                   Nut_per_100_grams = t5.Nut_per_100_grams,
                   Amount_g = t1.Amount_g,
                   Nutrition_Item = t6.Nutrition_Item,
                   DV = t6.DV,
                   Measurement = t6.Measurement,
                   Nutrition_Item_Id = t6.Nutrition_Item_Id

               };

            List<View_Nutrition_Panel> nuts = queryRI.ToList();
           
            var NutrientTable = new List<View_Nutrition_Panel>();
            decimal nutValue = 0;
            int nDV = 0;
            string nM = "";
            int servingSize = 1;
            


            //LOOP THROUGH VIEW AND GET ALL INGREDIENT NUTRIENTS THAT EQUAL ASSOCIATED INGRED IDS
            for (int i = 0; i < nuts.Count(); i++)
            {
                if (nuts[i].DV != null)
                {
                    nDV = (int)nuts[i].DV;
                }
                else
                {
                    nDV = 0;
                }


                nM = nuts[i].Measurement;
                if (nuts[i].Serving_Size != null)
                {
                    servingSize = (int)nuts[i].Serving_Size;
                }
                else
                {
                    servingSize = 1;
                }

                if (nuts[i].Nut_per_100_grams != null && nuts[i].Amount_g.ToString() != null)
                {
                    nutValue += ((decimal)nuts[i].Nut_per_100_grams / (decimal)100) * (decimal)nuts[i].Amount_g;
                }

                //IF THE NEXT NUTRIENT IS DIFFERENT, DUMP nutValue and reset 
                //CATCH OUT OF BOUNDS INDEX 
                try
                {
                    decimal tempAmountg = 0;

                    if (nuts[i].Nutrition_Item_Id != nuts[i + 1].Nutrition_Item_Id)
                    {
                        if (nDV != 0)
                        {
                            tempAmountg = ((nutValue / servingSize) / nDV) * (decimal)100;
                        }
                        else
                        {
                            tempAmountg = 0;

                        }

                        //BUILD LIST ITEM
                        var Fieldproperties = new View_Nutrition_Panel
                        {
                            Nutrition_Item = nuts[i].Nutrition_Item,
                            //DIVIDE BY 100 MULTIPLY BY INGRED AMOUNT
                            Nut_per_100_grams = nutValue,
                            //Amount_g = (nutValue / servingSize) / (nDV),
                            Amount_g = tempAmountg,
                            Measurement = nM,

                        };
                        //ADD TO THE LIST
                        NutrientTable.Add(Fieldproperties);
                        //RESET BUTVALUE FOR NEXT ENTRY
                        nutValue = 0;
                    }

                }
                catch (Exception)
                {
                    decimal tempAmountg = 0;
                    //CAUGHT OUT OF INDEX (out of items in list above)
                    if (nDV != 0)
                    {
                        tempAmountg = ((nutValue / servingSize) / nDV) * (decimal)100;
                    }
                    else
                    {
                        tempAmountg = 0;

                    }
                    //BUILD LIST ITEM
                    var Fieldproperties = new View_Nutrition_Panel
                    {
                        Nutrition_Item = nuts[i].Nutrition_Item,
                        //DIVIDE BY 100 MULTIPLY BY INGRED AMOUNT
                        Nut_per_100_grams = nutValue,
                        //Amount_g = (nutValue / servingSize) / (nDV),
                        Amount_g = tempAmountg,
                        Measurement = nM,

                    };
                    //ADD TO THE LIST
                    NutrientTable.Add(Fieldproperties);
                    //RESET BUTVALUE FOR NEXT ENTRY
                    nutValue = 0;
                }

            }


            ViewBag.Nutrition = NutrientTable;


            //---------------------------------------------------
            //GET ALLERGENS LIST     
            //---------------------------------------------------
            
            var queryA =
               from t1 in _recitopiaDbContext.Recipe.AsQueryable()
               join t2 in _recitopiaDbContext.Recipe_Ingredients.AsQueryable() on t1.Recipe_Id equals t2.Recipe_Id into t2g
               from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t3 in _recitopiaDbContext.Ingredient_Components.AsQueryable() on t2.Ingredient_Id equals t3.Ingred_Id into t3g
               from t3 in t3g.DefaultIfEmpty()
               join t4 in _recitopiaDbContext.Components.AsQueryable() on t3.Comp_Id equals t4.Comp_Id into t4g
               from t4 in t4g.DefaultIfEmpty()
               where t1.Recipe_Id == id
               orderby t4.Component_Name
               select new View_All_Recipe_Components()
               {
                   Recipe_Id = t1.Recipe_Id,
                   Customer_Id = t1.Customer_Id,
                   Recipe_Name = t1.Recipe_Name,
                   Ingredient_Id = t2.Ingredient_Id,
                   Amount_g = t2.Amount_g,
                   Comp_Id = t3.Comp_Id,
                   Component_Name = t4.Component_Name,
                   
               };


            List<View_All_Recipe_Components> TempV = queryA.ToList();

            //build allergen list to populate previously added items
            List<string> comList = new List<string>();

            foreach (View_All_Recipe_Components comp in TempV)
            {
                comList.Add(comp.Component_Name);
            }
            comList = comList.Distinct().ToList();
            ViewBag.RComponents = comList;

            //---------------------------------------------------
            //GET INGREDIENT LIST FOR NUTRITION PANEL - BUILD STRING
            //---------------------------------------------------

            var queryIL =
               from t1 in _recitopiaDbContext.Recipe_Ingredients.AsQueryable()
               join t2 in _recitopiaDbContext.Recipe.AsQueryable() on t1.Recipe_Id equals t2.Recipe_Id into t2g
               from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t3 in _recitopiaDbContext.Ingredient.AsQueryable() on t1.Ingredient_Id equals t3.Ingredient_Id
               where t1.Recipe_Id == id
               orderby t1.Amount_g descending
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

            List<View_All_Recipe_Ingredients> recipe_ing = queryIL.ToList();

            //build ingredient list to populate panel  
            string paneList = "";

            List<string> cols = new List<string>();

            foreach (View_All_Recipe_Ingredients comp in recipe_ing)
            {
                if (comp.Package == false)
                {
                    cols.Add(comp.Ingred_Comp_name);
                }
            }
            cols = cols.Distinct().ToList();

            for (int i = 0; i < cols.Count(); i++)
            {
                if (cols[i] != null)
                    if (i == 0)
                    {
                        paneList += cols[i];
                    }
                    else
                    {
                        paneList += ", " + cols[i];
                    }
            }

            ViewBag.IComponents = paneList;

            //----------------------------------------------------

            return View(recipe);
        }

        // GET: Recipes/Create
        public ActionResult Create()
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            ViewBag.Category_Id = new SelectList(_recitopiaDbContext.Meal_Category.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Category_Name), "Category_Id", "Category_Name");
            ViewBag.SS_Id = new SelectList(_recitopiaDbContext.Serving_Sizes.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Serving_Size), "SS_Id", "Serving_Size", 1);

            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] Recipe recipe)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));

            if (ModelState.IsValid)
            {
               
                
                recipe.Customer_Id = CustomerId;

                _recitopiaDbContext.Recipe.Add(recipe);
                _recitopiaDbContext.SaveChanges();

                //DIRECT LINQ TO DB REPLACING STORED PROCEDURE
                Recipe recipeFind = _recitopiaDbContext.Recipe.Where(i => i.Recipe_Id == recipe.Recipe_Id).Single();

                recipeFind.LastModified = DateTime.UtcNow;

                _recitopiaDbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Category_Id = new SelectList(_recitopiaDbContext.Meal_Category.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Category_Name), "Category_Id", "Category_Name", recipe.Category_Id);
            ViewBag.SS_ID = new SelectList(_recitopiaDbContext.Serving_Sizes.Where(m => m.Customer_Id == CustomerId).OrderByDescending(m => m.Serving_Size), "SS_Id", "Serving_Size", recipe.SS_Id);
            return View(recipe);
        }

        // GET: Recipes/Edit/5
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
            Recipe recipe = _recitopiaDbContext.Recipe.Find(id);

            if (recipe == null)
            {
                return NotFound();
            }
            ViewBag.Recipe_Name = recipe.Recipe_Name;

            ViewBag.Category_Id = new SelectList(_recitopiaDbContext.Meal_Category.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Category_Name), "Category_Id", "Category_Name", recipe.Category_Id);
            ViewBag.SS_Id = new SelectList(_recitopiaDbContext.Serving_Sizes.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Serving_Size), "SS_Id", "Serving_Size", recipe.SS_Id);

            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromForm] Recipe recipe)
        {
            
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (ModelState.IsValid)
            {
                recipe.Customer_Id = CustomerId;
                _recitopiaDbContext.Entry(recipe).State = EntityState.Modified;
                _recitopiaDbContext.SaveChanges();

                //DIRECT LINQ TO DB REPLACING STORED PROCEDURE
                Recipe recipeFind = _recitopiaDbContext.Recipe.Where(i => i.Recipe_Id == recipe.Recipe_Id).Single();

                recipeFind.LastModified = DateTime.UtcNow;

                _recitopiaDbContext.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.Category_Id = new SelectList(_recitopiaDbContext.Meal_Category.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Category_Name), "Category_Id", "Category_Name", recipe.Category_Id);
            ViewBag.SS_Id = new SelectList(_recitopiaDbContext.Serving_Sizes.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Serving_Size), "SS_Id", "Serving_Size", recipe.SS_Id);

            return View(recipe);
        }

        // GET: Recipes/Delete/5
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
            Recipe recipe = _recitopiaDbContext.Recipe.Find(id);

            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Recipe recipe = _recitopiaDbContext.Recipe.Find(id);

            try
            {
               
                _recitopiaDbContext.Recipe.Remove(recipe);

                _recitopiaDbContext.SaveChanges();

                return RedirectToAction("Index");
            }
           
            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(recipe);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "There are associated ingredients that need to be removed before you can delete this recipe.";
                return View(recipe);
            }


        }
        public ActionResult CreateCopy(int? id)
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

            Recipe recipe = _recitopiaDbContext.Recipe.Find(id);

            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }
        [HttpPost, ActionName("CreateCopy")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCopyConfirmed(int id)
        {

            Recipe recipe = _recitopiaDbContext.Recipe.Find(id);
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            try
            {

                //ADD RECIPE COPY
                Recipe recipe_new = new Recipe();
                    recipe_new.Recipe_Name = recipe.Recipe_Name + " - COPY";
                    recipe_new.Category_Id = recipe.Category_Id;
                    recipe_new.Gluten_Free = recipe.Gluten_Free;
                    recipe_new.SKU = recipe.SKU;
                    recipe_new.UPC = recipe.UPC;
                    recipe_new.Notes = recipe.Notes;
                    recipe_new.LaborCost = recipe.LaborCost;
                    recipe_new.SS_Id = recipe.SS_Id;
                    recipe_new.Customer_Id = recipe.Customer_Id;

                _recitopiaDbContext.Recipe.Add(recipe_new);
                _recitopiaDbContext.SaveChanges();

                //NEED TO ADD GROUP ID LATER
                var recipe_ingredients = _recitopiaDbContext.Recipe_Ingredients.Where(m => m.Recipe_Id == recipe.Recipe_Id && m.Customer_Id == CustomerId).ToList();

                foreach (Recipe_Ingredients thing in recipe_ingredients)
                {
                    Recipe_Ingredients recipe_ingredients_new = new Recipe_Ingredients()
                    {
                        Recipe_Id = recipe_new.Recipe_Id,
                        Ingredient_Id = thing.Ingredient_Id,
                        Amount_g = thing.Amount_g,
                        Customer_Id = thing.Customer_Id

                    };
                    //UPDATE DB - INSERT
                    _recitopiaDbContext.Recipe_Ingredients.Add(recipe_ingredients_new);
                    _recitopiaDbContext.SaveChanges();

                }

                return RedirectToAction("Index");
            }
            
            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(recipe);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 3";
                return View(recipe);
            }


        }
    }
}
