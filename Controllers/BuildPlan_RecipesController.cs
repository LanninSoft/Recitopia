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
    public class BuildPlan_RecipesController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public BuildPlan_RecipesController(RecitopiaDBContext recitopiaDbContext)
        {
            _recitopiaDbContext = recitopiaDbContext;
        }
        [Authorize]
        // GET: Order_Recipes
        public async Task<IActionResult> Index(int planId)
        {

            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null || customerGuid.Trim() == "")
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            var order = await _recitopiaDbContext.BuildPlan.FindAsync(planId);

            var orderItems = await _recitopiaDbContext.BuildPlan_Recipes
                .Include(ri => ri.BuildPlan)
                .Where(ri => ri.Customer_Guid == customerGuid && ri.BuildPlan_Id == planId)
                .OrderBy(ri => ri.BuildPlan.Plan_Name)
                .ToListAsync();

            ViewBag.OrderName = order.Plan_Name;
            ViewBag.PlanNameID = planId;
            ViewBag.Amount = orderItems.Sum(ri => ri.Amount);


            return View(orderItems);
        }
        [HttpGet]
        public async Task<JsonResult> GetData(int planId)
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");
           
            //BUILD OUT ORDER RECIPES ITEMS
            var orderItemDetails =
                 await (from rp in _recitopiaDbContext.BuildPlan_Recipes
                        join r in _recitopiaDbContext.Recipe
                        on rp.Recipe_Id equals r.Recipe_Id
                        join o in _recitopiaDbContext.BuildPlan
                        on rp.BuildPlan_Id equals o.BuildPlan_Id
                        where rp.BuildPlan_Id == planId && r.Customer_Guid == customerGuid
                        orderby o.Plan_Name
                        select new View_Angular_BuildPlan_Item_Details()
                        {
                            Id = rp.Id,
                            Customer_Guid = rp.Customer_Guid,
                            BuildPlan_Id = rp.BuildPlan_Id,
                            Recipe_Name = r.Recipe_Name,
                            Recipe_Id = rp.Recipe_Id,
                            Amount = rp.Amount
                        })
                .ToListAsync();

           
            return orderItemDetails != null
                ? Json(orderItemDetails)
                : Json(new { Status = "Failure" });
        }

        [HttpGet]
        public async Task<JsonResult> GetDataCreate(int plan_Id)
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            var usedRecipesforOrder = await _recitopiaDbContext.BuildPlan_Recipes
                .Where(m => m.Customer_Guid == customerGuid && m.BuildPlan_Id == plan_Id)
                .Select(ri => new View_Angular_BuildPlan_Item_Details()
                {
                    Recipe_Id = ri.Recipe_Id
                })
                .ToListAsync();


            List<int> RecipeIdList = new List<int>();

            foreach (View_Angular_BuildPlan_Item_Details thing in usedRecipesforOrder)
            {
                RecipeIdList.Add(thing.Recipe_Id);
            }


            var filterRecipes = await _recitopiaDbContext.Recipe
                .Where(f => !RecipeIdList.Contains(f.Recipe_Id) && f.Customer_Guid == customerGuid && f.isArchived == false)
                .OrderBy(i => i.Recipe_Name)
                .Select(ri => new View_Angular_BuildPlan_Item_Details()
                {
                    Recipe_Id = ri.Recipe_Id,
                    Customer_Guid = ri.Customer_Guid,
                    Recipe_Name = ri.Recipe_Name,
                    Amount = 0
                })
                .ToListAsync();


            return filterRecipes != null
                 ? Json(filterRecipes)
                 : Json(new { Status = "Failure" });
        }
        [HttpPost]
        public async Task<JsonResult> CreatePlanRecipes([FromBody]List<View_Angular_BuildPlan_Item_Details> planRecipesDetails)
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");
            var orderID = 0;

            foreach (View_Angular_BuildPlan_Item_Details detail in planRecipesDetails)
            {
                orderID = detail.BuildPlan_Id;

                if (detail.Amount > 0)
                {
                    var orderRecipe = new BuildPlan_Recipes();

                    orderRecipe.Amount = detail.Amount;
                    orderRecipe.Recipe_Id = detail.Recipe_Id;
                    orderRecipe.BuildPlan_Id = detail.BuildPlan_Id;
                    orderRecipe.Customer_Guid = customerGuid;

                    //Lets see if it's already in the db
                    var checkOrderRecipes = await _recitopiaDbContext.BuildPlan_Recipes
                        .Where(m => m.Customer_Guid == customerGuid && m.BuildPlan_Id == detail.BuildPlan_Id && m.Recipe_Id == detail.Recipe_Id)
                        .FirstOrDefaultAsync();

                    if (checkOrderRecipes == null)
                    {
                        await _recitopiaDbContext.BuildPlan_Recipes.AddAsync(orderRecipe);
                        await _recitopiaDbContext.SaveChangesAsync();
                    }


                }


            }


            //return Json("Success");
            return Json(orderID);
            //return RedirectToAction("Index", new { recipeId = recipeID });

        }
        [HttpPost]
        public async Task<JsonResult> UpdateFromAngularController([FromBody]List<View_Angular_BuildPlan_Item_Details> planItemsDetails)
        {
            var orderId = planItemsDetails.First().BuildPlan_Id;

            foreach (View_Angular_BuildPlan_Item_Details detail in planItemsDetails)
            {

                var orderItem = await _recitopiaDbContext.BuildPlan_Recipes
                        .SingleAsync(i => i.Id == detail.Id);

                if (detail.Amount >= 0)
                {
                    orderItem.Amount = detail.Amount;

                }
                else
                {
                    orderItem.Amount = 0;

                }
                await _recitopiaDbContext.SaveChangesAsync();
            }

            

            return Json(planItemsDetails);
        }
        // GET: Order_Recipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order_Recipes = await _recitopiaDbContext.BuildPlan_Recipes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order_Recipes == null)
            {
                return NotFound();
            }

            return View(order_Recipes);
        }

        // GET: Order_Recipes/Create
        public async Task<IActionResult> Create(int planId)
        {
            var order = await _recitopiaDbContext.BuildPlan.FindAsync(planId);

            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null || customerGuid.Trim() == "")
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            //Assign to temp local to put on view
            ViewBag.OrderName = order.Plan_Name;
            ViewBag.BuildPlan_Id = planId;

            //CREATE LIST OF PREVIOUS ADDED INGREDIENTS
            var planItems =
                 await (from rp in _recitopiaDbContext.BuildPlan_Recipes
                        join r in _recitopiaDbContext.Recipe
                        on rp.Recipe_Id equals r.Recipe_Id
                        join o in _recitopiaDbContext.BuildPlan
                        on rp.BuildPlan_Id equals o.BuildPlan_Id
                        where rp.BuildPlan_Id == planId && r.Customer_Guid == customerGuid
                        orderby o.Plan_Name
                        select new BuildPlan_Recipes()
                        {
                            Id = rp.Id,
                            Customer_Guid = rp.Customer_Guid,
                            BuildPlan_Id = rp.BuildPlan_Id,
                            Recipe_Name = r.Recipe_Name,
                            Recipe_Id = rp.Recipe_Id,
                            Amount = rp.Amount
                        })
                .ToListAsync();

        
            //build list to populate previously added items
            List<string> itemList = new List<string>();

            foreach (BuildPlan_Recipes orderItem in planItems)
            {
                if (orderItem.Amount.ToString() != null)
                {
                    itemList.Add(orderItem.Recipe_Name + "/" + orderItem.Amount.ToString() + "cnt");
                }
                else
                {
                    itemList.Add(orderItem.Recipe_Name + "/" + 0.ToString() + "cnt");
                }
            }

            ViewBag.Recipes = itemList;

            return View();
        }

        // POST: Order_Recipes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] BuildPlan_Recipes plan_Recipes)
        {
            if (ModelState.IsValid)
            {
                _recitopiaDbContext.Add(plan_Recipes);
                await _recitopiaDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plan_Recipes);
        }


        // GET: Order_Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plan_Recipes = await _recitopiaDbContext.BuildPlan_Recipes.FindAsync(id);
            if (plan_Recipes == null)
            {
                return NotFound();
            }
            return View(plan_Recipes);
        }

        // POST: Order_Recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] BuildPlan_Recipes plan_Recipes)
        {
            if (id != plan_Recipes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _recitopiaDbContext.Update(plan_Recipes);
                    await _recitopiaDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Order_RecipesExists(plan_Recipes.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(plan_Recipes);
        }

        // GET: Order_Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var planItems =
                 await (from rp in _recitopiaDbContext.BuildPlan_Recipes
                        join r in _recitopiaDbContext.Recipe
                        on rp.Recipe_Id equals r.Recipe_Id
                        join o in _recitopiaDbContext.BuildPlan
                        on rp.BuildPlan_Id equals o.BuildPlan_Id
                        where rp.Id == id 
                        select new BuildPlan_Recipes()
                        {
                            Id = rp.Id,
                            Customer_Guid = rp.Customer_Guid,
                            BuildPlan_Id = rp.BuildPlan_Id,
                            Recipe_Name = r.Recipe_Name,
                            Recipe_Id = rp.Recipe_Id,
                            Amount = rp.Amount
                        })
                .FirstOrDefaultAsync();
  
            if (planItems == null)
            {
                return NotFound();
            }

            return View(planItems);
        }

        // POST: Order_Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plan_Recipes = await _recitopiaDbContext.BuildPlan_Recipes.FindAsync(id);

            _recitopiaDbContext.BuildPlan_Recipes.Remove(plan_Recipes);

            await _recitopiaDbContext.SaveChangesAsync();
            
            return RedirectToAction("Index", new { planID = plan_Recipes.BuildPlan_Id });
        }

        private bool Order_RecipesExists(int id)
        {
            return _recitopiaDbContext.BuildPlan_Recipes.Any(e => e.Id == id);
        }
    }
}
