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

    public class IngredientsController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public IngredientsController(RecitopiaDBContext recitopiaDbContext)
        {
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
        }

        [Authorize]
        // GET: Ingredients
        public async Task<ActionResult> Index()
        {
            int customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));

            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            var ingredients = await _recitopiaDbContext.Ingredient
                .Include(i => i.Vendor)
                .Where(i => i.Customer_Id == customerId)
                .OrderBy(i => i.Ingred_name)
                .ToListAsync();

            return View(ingredients);
        }

        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            int customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            //BUILD VIEW FOR ANGULARJS RENDERING

            var ingredients = await _recitopiaDbContext.Ingredient
                .Include(ri => ri.Vendor)
                .Where(ri => ri.Customer_Id == customerId)
                .Select(ri => new View_Angular_Ingredients_Details()
                {
                    Ingredient_Id = ri.Ingredient_Id,
                    Customer_Id = ri.Customer_Id,
                    Ingred_name = ri.Ingred_name,
                    Cost_per_lb = ri.Cost_per_lb,
                    Cost = ri.Cost,
                    Package = ri.Package,
                    Vendor_Name = ri.Vendor.Vendor_Name,
                })
                .ToListAsync();


            return ingredients != null
                 ? Json(ingredients)
                 : Json(new { Status = "Failure" });
        }
        // GET: Ingredients/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            int customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Ingredient ingredient = await _recitopiaDbContext.Ingredient.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            //ASSOCIATED NUTRITION
            var ingredientNutrients = await _recitopiaDbContext.Ingredient_Nutrients
              .Include(ri => ri.Ingredients)
              .Include(ri => ri.Nutrition)
               .Where(ri => ri.Customer_Id == customerId && ri.Ingred_Id == id)
              .OrderBy(ri => ri.Nutrition.Nutrition_Item)
              .Select(ri => new View_All_Ingredient_Nutrients()
              {
                  Id = ri.Id,
                  Customer_Id = ri.Customer_Id,
                  Ingred_Id = ri.Ingred_Id,
                  Nutrition_Item_Id = ri.Nutrition_Item_Id,
                  Nut_per_100_grams = ri.Nut_per_100_grams,
                  Ingred_name = ri.Ingredients.Ingred_name,
                  Nutrition_Item = ri.Nutrition.Nutrition_Item,
              })
              .ToListAsync();


            ViewBag.IngredientNutritions = ingredientNutrients;

            //get recipe name from db and pass in viewbag
            Ingredient ingredient2 = await _recitopiaDbContext.Ingredient.FindAsync(ingredient.Ingredient_Id);

            //---------------------------------------------------
            //GET ALLERGENS            
            var ingredientComponents = await _recitopiaDbContext.Ingredient_Components
               .Include(ri => ri.Ingredients)
               .Include(ri => ri.Components)
               .Where(ri => ri.Customer_Id == customerId && ri.Ingred_Id == id)
               .Select(ri => new View_All_Ingredient_Components()
               {
                   Id = ri.Id,
                   Customer_Id = ri.Customer_Id,
                   Ingred_Id = ri.Ingred_Id,
                   Comp_Id = ri.Comp_Id,
                   Ingred_name = ri.Ingredients.Ingred_name,
                   Component_Name = ri.Components.Component_Name
               })
               .ToListAsync();

            //build list to populate previously added items
            List<string> comList = new List<string>();

            foreach (View_All_Ingredient_Components comp in ingredientComponents)
            {
                comList.Add(comp.Component_Name);

            }
            comList = comList.Distinct().ToList();

            ViewBag.RComponents = comList;
         

            //Assign to temp local to put on view
            ViewBag.IngredientName = ingredient2.Ingred_name;
            ViewBag.Ing_Id = ingredient2.Ingredient_Id;

            return View(ingredient);
        }

        // GET: Ingredients/Create
        public async Task<ActionResult> Create()
        {
            int customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            ViewBag.Vendor_Id = new SelectList(await _recitopiaDbContext.Vendor.Where(m => m.Customer_Id == customerId).OrderBy(m => m.Vendor_Name).ToListAsync(), "Vendor_Id", "Vendor_Name");
            return View();
        }

        // POST: Ingredients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] Ingredient ingredient)
        {
            int customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (ModelState.IsValid)
            {
                ingredient.Customer_Id = customerId;
                await _recitopiaDbContext.Ingredient.AddAsync(ingredient);
                await _recitopiaDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Vendor_Id = new SelectList(await _recitopiaDbContext.Vendor.Where(m => m.Customer_Id == customerId).OrderBy(m => m.Vendor_Name).ToListAsync(), "Vendor_Id", "Vendor_Name", ingredient.Vendor_Id);
            return View(ingredient);
        }

        // GET: Ingredients/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }

            int customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            Ingredient ingredient = await _recitopiaDbContext.Ingredient.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            //get recipe name from db and pass in viewbag
            Ingredient ingredient2 = await _recitopiaDbContext.Ingredient.FindAsync(ingredient.Ingredient_Id);

            //Assign to temp local to put on view
            ViewBag.IngredientName = ingredient2.Ingred_name;
            ViewBag.Ing_Id = ingredient2.Ingredient_Id;

            ViewBag.Vendor_Id = new SelectList(await _recitopiaDbContext.Vendor.Where(m => m.Customer_Id == customerId).OrderBy(m => m.Vendor_Name).ToListAsync(), "Vendor_Id", "Vendor_Name", ingredient.Vendor_Id);
            return View(ingredient);
        }

        // POST: Ingredients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([FromForm] Ingredient ingredient)
        {

            int customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            if (ModelState.IsValid)
            {
                ingredient.Customer_Id = customerId;
                _recitopiaDbContext.Entry(ingredient).State = EntityState.Modified;
                await _recitopiaDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Vendor_Id = new SelectList(await _recitopiaDbContext.Vendor.Where(m => m.Customer_Id == customerId).OrderBy(m => m.Vendor_Name).ToListAsync(), "Vendor_Id", "Vendor_Name", ingredient.Vendor_Id);
            return View(ingredient);
        }

        // GET: Ingredients/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Ingredient ingredient = await _recitopiaDbContext.Ingredient.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            //get recipe name from db and pass in viewbag
            Ingredient ingredient2 = await _recitopiaDbContext.Ingredient.FindAsync(ingredient.Ingredient_Id);

            //Assign to temp local to put on view
            ViewBag.IngredientName = ingredient2.Ingred_name;
            ViewBag.Ing_Id = ingredient2.Ingredient_Id;

            return View(ingredient);
        }

        // POST: Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Ingredient ingredient = await _recitopiaDbContext.Ingredient.FindAsync(id);

            try
            {

                _recitopiaDbContext.Ingredient.Remove(ingredient);
                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(ingredient);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "There are associated Nutrition Items that need to be removed before you can delete this recipe.";
                return View(ingredient);
            }


        }
        public async Task<ActionResult> CreateCopy(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }


            Ingredient ingredient = await _recitopiaDbContext.Ingredient.FindAsync(id);

            if (ingredient == null)
            {
                return NotFound();
            }
            return View(ingredient);
        }
        [HttpPost, ActionName("CreateCopy")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCopyConfirmed(int id)
        {

            Ingredient ingredient = await _recitopiaDbContext.Ingredient.FindAsync(id);
            int customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            try
            {

                //ADD INGREDIENT COPY
                Ingredient ingredient_new = new Ingredient();
                ingredient_new.Ingred_name = "Copy - " + ingredient.Ingred_name;
                ingredient_new.Ingred_Comp_name = ingredient.Ingred_Comp_name;
                ingredient_new.Cost_per_lb = ingredient.Cost_per_lb;
                ingredient_new.Vendor_Id = ingredient.Vendor_Id;
                ingredient_new.Website = ingredient.Website;
                ingredient_new.Notes = ingredient.Notes;
                ingredient_new.Packaging = ingredient.Packaging;
                ingredient_new.Brand = ingredient.Brand;
                ingredient_new.Package = ingredient.Package;
                ingredient_new.Cost = ingredient.Cost;
                ingredient_new.Customer_Id = ingredient.Customer_Id;

                await _recitopiaDbContext.Ingredient.AddAsync(ingredient_new);
                await _recitopiaDbContext.SaveChangesAsync();

                //COPY INGREDIENT_NUTRITION RELATIONSHIPS
                var ingredient_Nutrient = await _recitopiaDbContext.Ingredient_Nutrients.Where(m => m.Ingred_Id == ingredient.Ingredient_Id && m.Customer_Id == customerId).ToListAsync();

                foreach (Ingredient_Nutrients ingredientNutrient in ingredient_Nutrient)
                {
                    Ingredient_Nutrients ingredient_nutrient_new = new Ingredient_Nutrients()
                    {
                        Ingred_Id = ingredient_new.Ingredient_Id,
                        Nutrition_Item_Id = ingredientNutrient.Nutrition_Item_Id,
                        Nut_per_100_grams = ingredientNutrient.Nut_per_100_grams,
                        Customer_Id = ingredientNutrient.Customer_Id

                    };
                    //UPDATE DB - INSERT
                    await _recitopiaDbContext.Ingredient_Nutrients.AddAsync(ingredient_nutrient_new);
                    await _recitopiaDbContext.SaveChangesAsync();

                }
                //COPY INGREDIENT_COMPONENTS RELATIONSHIPS
                var ingredient_component = await _recitopiaDbContext.Ingredient_Components.Where(m => m.Ingred_Id == ingredient.Ingredient_Id && m.Customer_Id == customerId).ToListAsync();

                foreach (Ingredient_Components ingredientComponent in ingredient_component)
                {
                    Ingredient_Components ingredient_component_new = new Ingredient_Components()
                    {
                        Ingred_Id = ingredient_new.Ingredient_Id,
                        Comp_Id = ingredientComponent.Comp_Id,
                        Customer_Id = ingredientComponent.Customer_Id

                    };
                    //UPDATE DB - INSERT
                    await _recitopiaDbContext.Ingredient_Components.AddAsync(ingredient_component_new);
                    await _recitopiaDbContext.SaveChangesAsync();

                }
                return RedirectToAction("Index");
            }

            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(ingredient);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 3";
                return View(ingredient);
            }


        }

    }
}
