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
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            var recipes = await _recitopiaDbContext.Recipe
                .Include(r => r.Meal_Category)
                .Where(r => r.Customer_Guid == customerGuid)
                .OrderBy(r => r.Recipe_Name)
                .ToListAsync();

            return recipes != null ? View(recipes) : View();
        }
        
        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            //BUILD VIEW FOR ANGULARJS RENDERING
            var recipes = await _recitopiaDbContext.Recipe
               .Include(ri => ri.Meal_Category)
               .Include(ri => ri.Serving_Sizes)
               .Where(ri => ri.Customer_Guid == customerGuid)
               .OrderBy(ri => ri.Recipe_Name)
               .Select(ri => new View_Angular_Recipe_Details()
               {
                   Recipe_Id = ri.Recipe_Id,
                   Customer_Guid = ri.Customer_Guid,
                   Recipe_Name = ri.Recipe_Name,
                   Category_Id = ri.Category_Id,
                   Category_Name = ri.Meal_Category.Category_Name,
                   Gluten_Free = ri.Gluten_Free,
                   UPC = ri.UPC,
                   Notes = ri.Notes,
                   LaborCost = ri.LaborCost,
                   SS_Id = ri.SS_Id,
                   Serving_Size = ri.Serving_Sizes.Serving_Size,
                   LastModified = ri.LastModified,
               })
               .ToListAsync();


            return recipes != null
                 ? Json(recipes)
                 : Json(new { Status = "Failure" });


        }

        public async Task<ActionResult> Details(int id)
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            var recipe = await _recitopiaDbContext.Recipe
                .Include(r => r.Meal_Category)
                .Include(r => r.Serving_Sizes)
                .Where(r => r.Recipe_Id == id)
                .SingleAsync();

            if (recipe == null)
            {
                return NotFound();
            }

            //----------------------------------------------------
            //GET MAIN INGREDIENT LIST AND BUILD OUT COST COLUMN - 2 VIEWBAGS 
            //---------------------------------------------------

            //BUILD OUT RECIPE INGREDIENT ITEMS
            var recipeIngredients = await _recitopiaDbContext.Recipe_Ingredients
                            .Include(ri => ri.Recipe)
                            .Include(ri => ri.Ingredient)
                            .Where(ri => ri.Customer_Guid == customerGuid && ri.Recipe_Id == id)
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


          
            List<View_All_Recipe_Ingredients> recipe_ing2 = recipeIngredients;

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
            var nutritionPanel =
                 await (from ri in _recitopiaDbContext.Recipe_Ingredients
                        join r in _recitopiaDbContext.Recipe
                        on ri.Recipe_Id equals r.Recipe_Id
                        join ss in _recitopiaDbContext.Serving_Sizes
                        on r.SS_Id equals ss.SS_Id
                        join inn in _recitopiaDbContext.Ingredient_Nutrients
                        on ri.Ingredient_Id equals inn.Ingred_Id
                        join i in _recitopiaDbContext.Ingredient
                        on ri.Ingredient_Id equals i.Ingredient_Id
                        join n in _recitopiaDbContext.Nutrition
                        on inn.Nutrition_Item_Id equals n.Nutrition_Item_Id
                        where ri.Recipe_Id == id && n.ShowOnNutrientPanel == true && ri.Customer_Guid == customerGuid
                        orderby n.OrderOnNutrientPanel, n.Nutrition_Item
                        select new View_Nutrition_Panel
                        {
                            Customer_Guid = ri.Customer_Guid,
                            Recipe_Id = ri.Recipe_Id,
                            Recipe_Name = r.Recipe_Name,
                            Serving_Size = ss.Serving_Size,
                            Ingred_Id = ri.Ingredient_Id,
                            Ingred_name = i.Ingred_name,
                            ShowOnNutrientPanel = n.ShowOnNutrientPanel,
                            OrderOnNutrientPanel = n.OrderOnNutrientPanel,
                            Nut_per_100_grams = inn.Nut_per_100_grams,
                            Amount_g = ri.Amount_g,
                            Nutrition_Item = n.Nutrition_Item,
                            DV = n.DV,
                            Measurement = n.Measurement,
                            Nutrition_Item_Id = n.Nutrition_Item_Id
                        })
                .ToListAsync();


            List<View_Nutrition_Panel> nutritionPanelList = nutritionPanel;
           
            var NutrientTable = new List<View_Nutrition_Panel>();
            decimal nutValue = 0;
            int nDV = 0;
            string nM = "";
            int servingSize = 1;    

            //LOOP THROUGH VIEW AND GET ALL INGREDIENT NUTRIENTS THAT EQUAL ASSOCIATED INGRED IDS
            for (int i = 0; i < nutritionPanelList.Count(); i++)
            {
                if (nutritionPanelList[i].DV != null)
                {
                    nDV = (int)nutritionPanelList[i].DV;
                }
                else
                {
                    nDV = 0;
                }

                nM = nutritionPanelList[i].Measurement;
                if (nutritionPanelList[i].Serving_Size != null)
                {
                    servingSize = (int)nutritionPanelList[i].Serving_Size;
                }
                else
                {
                    servingSize = 1;
                }

                if (nutritionPanelList[i].Nut_per_100_grams != null && nutritionPanelList[i].Amount_g.ToString() != null)
                {
                    nutValue += ((decimal)nutritionPanelList[i].Nut_per_100_grams / (decimal)100) * (decimal)nutritionPanelList[i].Amount_g;
                }

                //IF THE NEXT NUTRIENT IS DIFFERENT, DUMP nutValue and reset 
                //CATCH OUT OF BOUNDS INDEX 
                try
                {
                    decimal tempAmountg = 0;

                    if (nutritionPanelList[i].Nutrition_Item_Id != nutritionPanelList[i + 1].Nutrition_Item_Id)
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
                            Nutrition_Item = nutritionPanelList[i].Nutrition_Item,
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
                        Nutrition_Item = nutritionPanelList[i].Nutrition_Item,
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
           
            var allergensList =
                 await (from r in _recitopiaDbContext.Recipe
                        join ri in _recitopiaDbContext.Recipe_Ingredients
                        on r.Recipe_Id equals ri.Recipe_Id
                        join ic in _recitopiaDbContext.Ingredient_Components
                        on ri.Ingredient_Id equals ic.Ingred_Id
                        join c in _recitopiaDbContext.Components
                        on ic.Comp_Id equals c.Comp_Id

                        where r.Recipe_Id == id && r.Customer_Guid == customerGuid
                        orderby c.Component_Name
                        select new View_All_Recipe_Components()
                        {
                            Recipe_Id = r.Recipe_Id,
                            Customer_Guid = r.Customer_Guid,
                            Recipe_Name = r.Recipe_Name,
                            Ingredient_Id = ri.Ingredient_Id,
                            Amount_g = ri.Amount_g,
                            Comp_Id = ic.Comp_Id,
                            Component_Name = c.Component_Name
                        })
                .ToListAsync();

            List<View_All_Recipe_Components> allergens = allergensList;

            //build allergen list to populate previously added items
            List<string> comList = new List<string>();

            foreach (View_All_Recipe_Components comp in allergens)
            {
                comList.Add(comp.Component_Name);
            }
            comList = comList.Distinct().ToList();
            ViewBag.RComponents = comList;

            //---------------------------------------------------
            //GET INGREDIENT LIST FOR NUTRITION PANEL - BUILD STRING
            //---------------------------------------------------
            var ingredients = await _recitopiaDbContext.Recipe_Ingredients
                            .Include(ri => ri.Recipe)
                            .Include(ri => ri.Ingredient)
                            .Where(ri => ri.Customer_Guid == customerGuid && ri.Recipe_Id == id)
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


            List<View_All_Recipe_Ingredients> recipeIngredient = ingredients;

            //build ingredient list to populate panel  
            string paneList = "";

            List<string> cols = new List<string>();

            foreach (View_All_Recipe_Ingredients comp in recipeIngredient)
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
        public async Task<ActionResult> Create()
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            ViewBag.Category_Id = new SelectList(await _recitopiaDbContext.Meal_Category.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Category_Name).ToListAsync(), "Category_Id", "Category_Name");
            ViewBag.SS_Id = new SelectList(await _recitopiaDbContext.Serving_Sizes.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Serving_Size).ToListAsync(), "SS_Id", "Serving_Size", 1);

            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] Recipe recipe)
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (ModelState.IsValid)
            {
               recipe.Customer_Guid = customerGuid;

                await _recitopiaDbContext.Recipe.AddAsync(recipe);
                await _recitopiaDbContext.SaveChangesAsync();

                //DIRECT LINQ TO DB REPLACING STORED PROCEDURE
                Recipe recipeFind = _recitopiaDbContext.Recipe.Where(i => i.Recipe_Id == recipe.Recipe_Id).Single();

                recipeFind.LastModified = DateTime.UtcNow;

                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.Category_Id = new SelectList(await _recitopiaDbContext.Meal_Category.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Category_Name).ToListAsync(), "Category_Id", "Category_Name", recipe.Category_Id);
            ViewBag.SS_ID = new SelectList(await _recitopiaDbContext.Serving_Sizes.Where(m => m.Customer_Guid == customerGuid).OrderByDescending(m => m.Serving_Size).ToListAsync(), "SS_Id", "Serving_Size", recipe.SS_Id);
            return View(recipe);
        }

        // GET: Recipes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (id == null)
            {
                return new StatusCodeResult(0);
            }

            Recipe recipe = await _recitopiaDbContext.Recipe.FindAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }
            ViewBag.Recipe_Name = recipe.Recipe_Name;

            ViewBag.Category_Id = new SelectList(await _recitopiaDbContext.Meal_Category.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Category_Name).ToListAsync(), "Category_Id", "Category_Name", recipe.Category_Id);
            ViewBag.SS_Id = new SelectList(await _recitopiaDbContext.Serving_Sizes.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Serving_Size).ToListAsync(), "SS_Id", "Serving_Size", recipe.SS_Id);

            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([FromForm] Recipe recipe)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (ModelState.IsValid)
            {
                recipe.Customer_Guid = customerGuid;

                _recitopiaDbContext.Entry(recipe).State = EntityState.Modified;
                await _recitopiaDbContext.SaveChangesAsync();

                //DIRECT LINQ TO DB REPLACING STORED PROCEDURE
                Recipe recipeFind = await _recitopiaDbContext.Recipe.Where(i => i.Recipe_Id == recipe.Recipe_Id).SingleAsync();

                recipeFind.LastModified = DateTime.UtcNow;

                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            ViewBag.Category_Id = new SelectList(await _recitopiaDbContext.Meal_Category.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Category_Name).ToListAsync(), "Category_Id", "Category_Name", recipe.Category_Id);
            ViewBag.SS_Id = new SelectList(await _recitopiaDbContext.Serving_Sizes.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Serving_Size).ToListAsync(), "SS_Id", "Serving_Size", recipe.SS_Id);

            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            Recipe recipe = await _recitopiaDbContext.Recipe.FindAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {

            Recipe recipe = await _recitopiaDbContext.Recipe.FindAsync(id);

            try
            {
               
                _recitopiaDbContext.Recipe.Remove(recipe);

                await _recitopiaDbContext.SaveChangesAsync();

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
        public async Task<ActionResult> CreateCopy(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }

            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            Recipe recipe = await _recitopiaDbContext.Recipe.FindAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }
        [HttpPost, ActionName("CreateCopy")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCopyConfirmed(int id)
        {

            Recipe recipe = await _recitopiaDbContext.Recipe.FindAsync(id);
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");


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
                    recipe_new.Customer_Guid = recipe.Customer_Guid;

                await _recitopiaDbContext.Recipe.AddAsync(recipe_new);
                await _recitopiaDbContext.SaveChangesAsync();

                //NEED TO ADD GROUP ID LATER
                var recipe_ingredients = await _recitopiaDbContext.Recipe_Ingredients.Where(m => m.Recipe_Id == recipe.Recipe_Id && m.Customer_Guid == customerGuid).ToListAsync();

                foreach (Recipe_Ingredients recipeIngredient in recipe_ingredients)
                {
                    Recipe_Ingredients recipe_ingredients_new = new Recipe_Ingredients()
                    {
                        Recipe_Id = recipe_new.Recipe_Id,
                        Ingredient_Id = recipeIngredient.Ingredient_Id,
                        Amount_g = recipeIngredient.Amount_g,
                        Customer_Guid = recipeIngredient.Customer_Guid

                    };
                    //UPDATE DB - INSERT
                    _recitopiaDbContext.Recipe_Ingredients.Add(recipe_ingredients_new);
                    await _recitopiaDbContext.SaveChangesAsync();

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
