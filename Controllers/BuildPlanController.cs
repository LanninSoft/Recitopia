using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recitopia.Data;
using Recitopia.Models;

namespace Recitopia.Controllers
{
    public class BuildPlanController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public BuildPlanController(RecitopiaDBContext recitopiaDbContext)
        {
            _recitopiaDbContext = recitopiaDbContext;
        }
        [Authorize]
        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null || customerGuid.Trim() == "")
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            var orders = await _recitopiaDbContext.BuildPlan
                .Include(r => r.BuildPlan_Recipes)
                .Where(r => r.Customer_Guid == customerGuid)
                .OrderBy(r => r.Plan_Name)
                .ToListAsync();

            return orders != null ? View(orders) : View();
        }
        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            var orders = await _recitopiaDbContext.BuildPlan
                .Include(r => r.BuildPlan_Recipes)
                .Where(r => r.Customer_Guid == customerGuid)
                .OrderBy(r => r.Plan_Name)
                .ToListAsync();


            return orders != null
                 ? Json(orders)
                 : Json(new { Status = "Failure" });


        }
        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null || customerGuid.Trim() == "")
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (id == null)
            {
                return NotFound();
            }

            var plan = await _recitopiaDbContext.BuildPlan
                .FirstOrDefaultAsync(m => m.BuildPlan_Id == id);
            if (plan == null)
            {
                return NotFound();
            }

            //GET DISTINCT RECIPE LIST WITH TOTAL COUNT, SUBTOTAL COST
            var planRecipes =
                 await (from br in _recitopiaDbContext.BuildPlan_Recipes
                        join r in _recitopiaDbContext.Recipe
                        on br.Recipe_Id equals r.Recipe_Id
                        where br.BuildPlan_Id == id && br.Customer_Guid == customerGuid
                        select new BuildPlan_Recipes()
                        {                            
                            Recipe_Id = br.Recipe_Id, 
                            Recipe_Name = r.Recipe_Name,
                            Amount = br.Amount
                        })
                .ToListAsync();

            var distinctPlanRecipes = planRecipes.Distinct();

            //RECIPE INGREDIENTS
            //Have distinct Recipe IDs, now loop through and summerize data for each and put in View_All_Plan_Recipes_Details then to viewbag
            List<View_All_Plan_Recipes_Details> mainPanel = new List<View_All_Plan_Recipes_Details>();

            foreach(BuildPlan_Recipes item in distinctPlanRecipes)
            {
                var recipeIngredients =
                 await (from ri in _recitopiaDbContext.Recipe_Ingredients
                        join i in _recitopiaDbContext.Ingredient
                        on ri.Ingredient_Id equals i.Ingredient_Id
                        join r in _recitopiaDbContext.Recipe
                        on ri.Recipe_Id equals r.Recipe_Id
                        where ri.Recipe_Id == item.Recipe_Id && ri.Customer_Guid == customerGuid
                        orderby ri.Recipe_Id
                        select new View_All_Recipe_Ingredients()
                        {
                            Recipe_Id = ri.Recipe_Id,
                            Recipe_Name = r.Recipe_Name,
                            Ingredient_Id = ri.Ingredient_Id,
                            Cost_per_lb = i.Cost_per_lb,
                            Amount_g = ri.Amount_g,
                            CountIt = item.Amount,
                            Cost = ((ri.Amount_g/(decimal)453.592) * i.Cost_per_lb) * item.Amount
                        })
                .ToListAsync();

                var sumGForRecipe = recipeIngredients.Sum(m => m.Amount_g);
                var sumCost = recipeIngredients.Sum(m => m.Cost);

                //RECIP PACKAGING
                var recipePackaging =
                 await (from ri in _recitopiaDbContext.Recipe_Packaging
                        join p in _recitopiaDbContext.Packaging
                        on ri.Package_Id equals p.Package_Id
                        join r in _recitopiaDbContext.Recipe
                        on ri.Recipe_Id equals r.Recipe_Id
                        where ri.Recipe_Id == item.Recipe_Id && ri.Customer_Guid == customerGuid
                        orderby ri.Recipe_Id
                        select new View_All_Recipe_Packaging()
                        {
                            Recipe_Id = ri.Recipe_Id,
                            Recipe_Name = r.Recipe_Name,
                            Package_Id = ri.Package_Id,
                            Weight = p.Weight,
                            Amount = ri.Amount,
                            CountIt = item.Amount,
                            Cost = (p.Cost * ri.Amount) * item.Amount
                        })
                .ToListAsync();                

                //ADD IT ALL TOGETHER
                sumCost += recipePackaging.Sum(m => m.Cost);

                //CREATE OBJECT
                var tempMainPanel = new View_All_Plan_Recipes_Details()
                {
                    Recipe_Name = item.Recipe_Name,
                    CountIt = item.Amount,
                    Cost = Math.Round((decimal)sumCost, 3)
                };

                //ADD TO MODEL FOR VIEWING
                mainPanel.Add(tempMainPanel);

            }

            ViewBag.MainRecipePanel = mainPanel.ToList();
            ViewBag.MainRecipePanelSubTotal = mainPanel.Sum(m => m.Cost);

            //LIST INGREDIENTS AND AMOUNTS AND COSTS

            //GET DISTINCT INGREDIENT LIST WITH TOTAL COUNT, SUBTOTAL COST
            var planIngredients =
                 await (from br in _recitopiaDbContext.BuildPlan_Recipes
                        join ri in _recitopiaDbContext.Recipe_Ingredients
                        on br.Recipe_Id equals ri.Recipe_Id
                        join i in _recitopiaDbContext.Ingredient
                        on ri.Ingredient_Id equals i.Ingredient_Id
                        where br.BuildPlan_Id == id && br.Customer_Guid == customerGuid
                        orderby i.Ingred_name
                        select new View_All_Recipe_Ingredients()
                        {
                            Recipe_Id = br.Recipe_Id,
                            Ingredient_Id = i.Ingredient_Id,
                            Amount_g = ri.Amount_g,
                            Ingred_name = i.Ingred_name,
                            Cost_per_lb = i.Cost_per_lb,
                            Cost = i.Cost,
                            CountIt = br.Amount,
                            SubTotalCostIngredients = ((ri.Amount_g * br.Amount)/ (decimal)453.592) * (decimal)i.Cost_per_lb,
                            Amount_lbs = (ri.Amount_g * br.Amount) / (decimal)453.592,
                            SubTotalGrams = ri.Amount_g * br.Amount

                        })
                .ToListAsync();
                        
            ViewBag.MainIngredientPanelCostTotal = Math.Round(planIngredients.Sum(m => m.SubTotalCostIngredients),3);
            ViewBag.MainIngredientPanellbsTotal = Math.Round(planIngredients.Sum(m => m.Amount_lbs), 3);
            ViewBag.MainIngredientPanelgramsTotal = Math.Round(planIngredients.Sum(m => m.SubTotalGrams), 3);

            //GROUP TABLE AND SUM
            var planIngredientsGrouped = planIngredients
            .GroupBy(a => a.Ingred_name)
            .Select(a => new View_All_Recipe_Ingredients()
            {
                SubTotalGrams = a.Sum(b => b.SubTotalGrams),
                Amount_lbs = a.Sum(b => b.Amount_lbs),
                SubTotalCostIngredients = a.Sum(b => b.SubTotalCostIngredients),
                Ingred_name = a.Key      
            })
            .ToList();

            ViewBag.MainIngredientPanel = planIngredientsGrouped;

            //LIST PACKAGING AND AMOUNTS AND COSTS

            //GET DISTINCT PACKAGING LIST WITH TOTAL COUNT, SUBTOTAL COST
            var planPackaging =
                 await (from br in _recitopiaDbContext.BuildPlan_Recipes
                        join rp in _recitopiaDbContext.Recipe_Packaging
                        on br.Recipe_Id equals rp.Recipe_Id
                        join p in _recitopiaDbContext.Packaging
                        on rp.Package_Id equals p.Package_Id
                        where br.BuildPlan_Id == id && br.Customer_Guid == customerGuid
                        orderby p.Package_Name
                        select new View_All_Recipe_Packaging()
                        {
                            Recipe_Id = br.Recipe_Id,
                            Package_Id = rp.Package_Id,
                            Package_Name = p.Package_Name,
                            Weight = p.Weight,
                            Amount = br.Amount,
                            Cost = p.Cost * br.Amount,
                            CountIt = rp.Amount * br.Amount,
                            SubTotalCostPackaging = (p.Cost * rp.Amount) * br.Amount,
                            Amount_lbs = (p.Weight * br.Amount) / (decimal)453.592,
                            SubTotalGrams = p.Weight * br.Amount


                        })
                .ToListAsync();

            ViewBag.MainPackagingPanelCostTotal = Math.Round(planPackaging.Sum(m => m.SubTotalCostPackaging), 3);
            ViewBag.MainPackagingPanellbsTotal = Math.Round(planPackaging.Sum(m => m.Amount_lbs), 3);
            ViewBag.MainPackagingPanelgramsTotal = Math.Round(planPackaging.Sum(m => m.SubTotalGrams), 3);

            //GROUP TABLE AND SUM
            var planPackagingGrouped = planPackaging
            .GroupBy(a => a.Package_Name)
            .Select(a => new View_All_Recipe_Packaging()
            {
                SubTotalGrams = a.Sum(b => b.SubTotalGrams),
                Amount_lbs = a.Sum(b => b.Amount_lbs),
                SubTotalCostPackaging = a.Sum(b => b.SubTotalCostPackaging),
                CountIt = a.Sum(b => b.CountIt),
                Package_Name = a.Key
            })
            .ToList();

            ViewBag.MainPackagingPanel = planPackagingGrouped;

            //GRAND TOTAL
            ViewBag.MainPackagingPanelCostGrandTotal = ViewBag.MainPackagingPanelCostTotal + ViewBag.MainIngredientPanelCostTotal ;

            ViewBag.MainPackagingPanellbsGrandTotal = ViewBag.MainPackagingPanellbsTotal + ViewBag.MainIngredientPanellbsTotal;

            return View(plan);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] BuildPlan order)
        {
            if (ModelState.IsValid)
            {
                var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

                order.Customer_Guid = customerGuid;
                order.LastModified = DateTime.UtcNow;
                order.CreateDate = DateTime.UtcNow;

                _recitopiaDbContext.Add(order);

                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Details", new { id = order.BuildPlan_Id });
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plan = await _recitopiaDbContext.BuildPlan.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }
            return View(plan);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] BuildPlan plan)
        {
            

            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (ModelState.IsValid)
            {
                try
                {
                    plan.LastModified = DateTime.UtcNow;
                    plan.Customer_Guid = customerGuid;

                    _recitopiaDbContext.Update(plan);
                    await _recitopiaDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(plan.BuildPlan_Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = plan.BuildPlan_Id });
            }
            return View(plan);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _recitopiaDbContext.BuildPlan
                .FirstOrDefaultAsync(m => m.BuildPlan_Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            //DELETE FROM ORDER RECIPES FIRST
            var orderItemsToDelete = await _recitopiaDbContext.BuildPlan_Recipes.Where(m => m.BuildPlan_Id == id).ToListAsync();
            if (orderItemsToDelete != null)
            {
                _recitopiaDbContext.BuildPlan_Recipes.RemoveRange(orderItemsToDelete);
                await _recitopiaDbContext.SaveChangesAsync();

            }            
            
            var order = await _recitopiaDbContext.BuildPlan.FindAsync(id);
            _recitopiaDbContext.BuildPlan.Remove(order);
            await _recitopiaDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _recitopiaDbContext.BuildPlan.Any(e => e.BuildPlan_Id == id);
        }
        public async Task<ActionResult> CreateCopy(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }

            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null || customerGuid.Trim() == "")
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            BuildPlan plan = await _recitopiaDbContext.BuildPlan.FindAsync(id);

            if (plan == null)
            {
                return NotFound();
            }
            return View(plan);
        }
        [HttpPost, ActionName("CreateCopy")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCopyConfirmed(int id)
        {

            BuildPlan plan = await _recitopiaDbContext.BuildPlan.FindAsync(id);

            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            try
            {
                //ADD RECIPE COPY
                var buildPlan = new BuildPlan()
                {
                    Plan_Name = plan.Plan_Name + "-copy",
                    PlanDate = plan.PlanDate,
                    NeedByDate = plan.NeedByDate,
                    FullFilled = plan.FullFilled,
                    Notes = plan.Notes,
                    CreateDate = DateTime.UtcNow,
                    Customer_Guid = customerGuid
                };

                await _recitopiaDbContext.BuildPlan.AddAsync(buildPlan);
                await _recitopiaDbContext.SaveChangesAsync();

                //GET PLAN RECIPES
                var planRecipes = await _recitopiaDbContext.BuildPlan_Recipes.Where(m => m.BuildPlan_Id == plan.BuildPlan_Id && m.Customer_Guid == customerGuid).ToListAsync();

                foreach (BuildPlan_Recipes planRecipe in planRecipes)
                {
                    BuildPlan_Recipes planIngredientsNew = new BuildPlan_Recipes()
                    {
                        Recipe_Id = planRecipe.Recipe_Id,
                        BuildPlan_Id = buildPlan.BuildPlan_Id,
                        Amount = planRecipe.Amount,
                        Customer_Guid = planRecipe.Customer_Guid,

                    };
                    //UPDATE DB - INSERT
                    _recitopiaDbContext.BuildPlan_Recipes.Add(planIngredientsNew);
                    await _recitopiaDbContext.SaveChangesAsync();

                }
                
                return RedirectToAction("Edit", new { id = buildPlan.BuildPlan_Id });
            }

            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(plan);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 3";
                return View(plan);
            }


        }

    }
}
