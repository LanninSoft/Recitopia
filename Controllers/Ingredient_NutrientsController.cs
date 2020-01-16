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
    public class Ingredient_NutrientsController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public Ingredient_NutrientsController(RecitopiaDBContext recitopiaDbContext)
        {
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
        }

        [Authorize]
        // GET: Ingredient_Nutrients
        public async Task<ActionResult> Index(int IngredID)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if  (customerGuid == null || customerGuid.Trim() == "")
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            var ingredient_Nutrients = await _recitopiaDbContext.Ingredient_Nutrients
                .Include(i => i.Ingredients)
                .Include(n => n.Nutrition)
                .Where(i => i.Ingred_Id == IngredID && i.Customer_Guid == customerGuid)
                .OrderBy(i => i.Nutrition.OrderOnNutrientPanel)
                .ThenBy(i => i.Nutrition.Nutrition_Item)
                .ToListAsync();

            
            //get Ingred name from db and pass in viewbag
            Ingredient ingredient = await _recitopiaDbContext.Ingredient.FindAsync(IngredID);

            //Assign to temp local to put on view
            ViewBag.IngredName = ingredient.Ingred_name;

            ViewBag.IngredNameID = IngredID;

            return View(ingredient_Nutrients.ToList());
        }
        [HttpGet]
        public async Task<JsonResult> GetData(int ingredId)
        {

            //BUILD VIEW FOR ANGULARJS RENDERING
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");
            var ingredientNutrients = await _recitopiaDbContext.Ingredient_Nutrients
              .Include(ri => ri.Ingredients)
              .Include(ri => ri.Nutrition)
              .Where(ri => ri.Customer_Guid == customerGuid && ri.Ingred_Id == ingredId)
              .OrderBy(ri => ri.Nutrition.OrderOnNutrientPanel)
              .ThenBy(i => i.Nutrition.Nutrition_Item)
              .Select(ri => new View_Angular_Ingredient_Nutrients_Details()
              {
                  Id = ri.Id,
                  Customer_Guid = ri.Customer_Guid,
                  Ingred_Id = ri.Ingred_Id,
                  Ingred_name = ri.Ingredients.Ingred_name,
                  Nutrition_Item_Id = ri.Nutrition_Item_Id,
                  Nutrition_Item = ri.Nutrition.Nutrition_Item,
                  Nut_per_100_grams = ri.Nut_per_100_grams
              })
              .ToListAsync();

            return ingredientNutrients != null
                ? Json(ingredientNutrients)
                : Json(new { Status = "Failure" });
        }
        // GET: Ingredient_Nutrients/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Ingredient_Nutrients ingredient_Nutrients = await _recitopiaDbContext.Ingredient_Nutrients.FindAsync(id);
            if (ingredient_Nutrients == null)
            {
                return NotFound();
            }
            return View(ingredient_Nutrients);
        }

        // GET: Ingredient_Nutrients/Create
        public async Task<ActionResult> Create(int IngredID)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");
            if  (customerGuid == null || customerGuid.Trim() == "")
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            //get recipe name from db and pass in viewbag
            Ingredient ingredient = await _recitopiaDbContext.Ingredient.FindAsync(IngredID);

            //Assign to temp local to put on view
            ViewBag.IngredName = ingredient.Ingred_name;
            ViewBag.IngredId = IngredID;

            ViewBag.Ingred_Id = new SelectList(await _recitopiaDbContext.Ingredient.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Ingred_name).ToListAsync(), "Ingredient_Id", "Ingred_name");
            ViewBag.Nutrition_Item_Id = new SelectList(await _recitopiaDbContext.Nutrition.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Nutrition_Item).ToListAsync(), "Nutrition_Item_Id", "Nutrition_Item");

            //BUILD VIEW FOR ALREADY ADDED NUTRIENTS
            var ingredientNutrients =  _recitopiaDbContext.Ingredient_Nutrients
                .Include(ri => ri.Ingredients)
                .Include(ri => ri.Nutrition)
                .Where(ri => ri.Customer_Guid == customerGuid && ri.Ingred_Id == IngredID)
                .Select(ri => new View_All_Ingredient_Nutrients()
                {
                    Id = ri.Id,
                    Customer_Guid = ri.Customer_Guid,
                    Ingred_Id = ri.Ingred_Id,
                    Ingred_name = ri.Ingredients.Ingred_name,
                    Nutrition_Item_Id= ri.Nutrition_Item_Id,
                    Nutrition_Item = ri.Nutrition.Nutrition_Item
                })
                .ToListAsync();

            //Get View and filter and sort            
            var TempV = ingredientNutrients.Result;

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
        public async Task<ActionResult> Create([FromForm] Ingredient_Nutrients ingredient_Nutrients, int RID)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");
            if  (customerGuid == null || customerGuid.Trim() == "")
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (RID > 0)
            {
                ViewBag.Ingred_Id = RID;
                ingredient_Nutrients.Ingred_Id = RID;
                ingredient_Nutrients.Customer_Guid = customerGuid;
                await  _recitopiaDbContext.Ingredient_Nutrients.AddAsync(ingredient_Nutrients);

                await _recitopiaDbContext.SaveChangesAsync();
                return RedirectToAction("Index", new { IngredID = ingredient_Nutrients.Ingred_Id });
            }

            ViewBag.Ingred_Id = RID;
            ViewBag.Nutrition_Item_Id = new SelectList(await _recitopiaDbContext.Nutrition.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Nutrition_Item).ToListAsync(), "Nutrition_Item_Id", "Nutrition_Item", ingredient_Nutrients.Nutrition_Item_Id);
            return View(ingredient_Nutrients);
        }

        // GET: Ingredient_Nutrients/Edit/5
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

            //get ingred id
            Ingredient_Nutrients ingredient_nutrients = await _recitopiaDbContext.Ingredient_Nutrients.FindAsync(id);
            if (ingredient_nutrients == null)
            {
                return NotFound();
            }

            //get name from db and pass in viewbag
            Ingredient ingredient = await _recitopiaDbContext.Ingredient.FindAsync(ingredient_nutrients.Ingred_Id);

            //Assign to temp local to put on view
            ViewBag.IngredName = ingredient.Ingred_name;
            ViewBag.IngredId = ingredient.Ingredient_Id;

            ViewBag.Ingred_Id = new SelectList(await _recitopiaDbContext.Ingredient.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Ingred_name).ToListAsync(), "Ingredient_Id", "Ingred_name", ingredient_nutrients.Ingred_Id);
            ViewBag.Nutrition_Item_Id = new SelectList(await _recitopiaDbContext.Nutrition.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Nutrition_Item).ToListAsync(), "Nutrition_Item_Id", "Nutrition_Item", ingredient_nutrients.Nutrition_Item_Id);
            return View(ingredient_nutrients);
        }

        // POST: Ingredient_Nutrients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([FromForm] Ingredient_Nutrients ingredient_Nutrients)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");
            if  (customerGuid == null || customerGuid.Trim() == "")
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (ModelState.IsValid)
            {
                ingredient_Nutrients.Customer_Guid = customerGuid;
                _recitopiaDbContext.Entry(ingredient_Nutrients).State = EntityState.Modified;
                await _recitopiaDbContext.SaveChangesAsync();
                return RedirectToAction("Index", new { IngredID = ingredient_Nutrients.Ingred_Id });
            }
            ViewBag.Ingred_Id = new SelectList(await _recitopiaDbContext.Ingredient.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Ingred_name).ToListAsync(), "Ingredient_Id", "Ingred_name", ingredient_Nutrients.Ingred_Id);
            ViewBag.Nutrition_Item_Id = new SelectList(await _recitopiaDbContext.Nutrition.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Nutrition_Item).ToListAsync(), "Nutrition_Item_Id", "Nutrition_Item", ingredient_Nutrients.Nutrition_Item_Id);
            return View(ingredient_Nutrients);
        }
       

        //UPDATE FROM ANGULAR
        [HttpPost]
        public async Task<JsonResult> UpdateFromAngularController([FromBody]IEnumerable<View_Angular_Ingredient_Nutrients_Details> jsonstring)
        {

            foreach (View_Angular_Ingredient_Nutrients_Details ingredientNutrient in jsonstring)
            {
                if (ingredientNutrient.Nut_per_100_grams > 0)
                {
                    Ingredient_Nutrients ingredient_nutrients = await _recitopiaDbContext.Ingredient_Nutrients.Where(i => i.Id == ingredientNutrient.Id).SingleAsync();

                    ingredient_nutrients.Nut_per_100_grams = ingredientNutrient.Nut_per_100_grams;

                    await _recitopiaDbContext.SaveChangesAsync();
                }

            }
            return Json(jsonstring);

        }
        
        // GET: Ingredient_Nutrients/Delete/5
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

            var Ingredients_Nutrients = await _recitopiaDbContext.Ingredient_Nutrients.Where(m => m.Id == id)
                .Include(r => r.Ingredients)
                .Include(r => r.Nutrition)
                .Where(m => m.Customer_Guid == customerGuid).ToListAsync();

            if (Ingredients_Nutrients == null)
            {
                return NotFound();
            }
            //GOT TO BE A BETTER WAY THAN BUILD THE MODEL BY HAND:/
            Ingredient_Nutrients rIngreds = new Ingredient_Nutrients();
            Ingredient tIngred = new Ingredient();
            Nutrition tNut = new Nutrition();

            foreach (Ingredient_Nutrients ingredientNutrient in Ingredients_Nutrients)
            {
                tIngred = ingredientNutrient.Ingredients;
                tNut = ingredientNutrient.Nutrition;

                rIngreds.Id = ingredientNutrient.Id;
                rIngreds.Ingred_Id = ingredientNutrient.Ingred_Id;
                rIngreds.Ingredients = tIngred;
                rIngreds.Nutrition_Item_Id = ingredientNutrient.Nutrition_Item_Id;
                rIngreds.Nutrition = tNut;
                rIngreds.Customer_Guid = ingredientNutrient.Customer_Guid;

            }           
            return View(rIngreds);
        }

        // POST: Ingredient_Nutrients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Ingredient_Nutrients ingredient_Nutrients = await _recitopiaDbContext.Ingredient_Nutrients.FindAsync(id);

            try
            {
                _recitopiaDbContext.Ingredient_Nutrients.Remove(ingredient_Nutrients);
                await _recitopiaDbContext.SaveChangesAsync();
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
        public async Task<ActionResult> PrimeIngredNutrient(int? id)
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
        [HttpPost, ActionName("PrimeIngredNutrient")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PrimeIngredNutrientConfirmed(int id)
        {

            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");
            if  (customerGuid == null || customerGuid.Trim() == "")
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            Ingredient ingredient = await _recitopiaDbContext.Ingredient.FindAsync(id);
           
            var nutrients = await _recitopiaDbContext.Nutrition.Where(m => m.ShowOnNutrientPanel == true && m.Customer_Guid == customerGuid).ToListAsync();

            foreach (Nutrition nutrient in nutrients)
            {
                Ingredient_Nutrients ingredient_nutrients = new Ingredient_Nutrients() {
                    Ingred_Id = ingredient.Ingredient_Id,
                    Nutrition_Item_Id = nutrient.Nutrition_Item_Id,
                    Nut_per_100_grams = 0,
                    Customer_Guid = ingredient.Customer_Guid

                };
                //UPDATE DB - INSERT
                await _recitopiaDbContext.Ingredient_Nutrients.AddAsync(ingredient_nutrients);
                await _recitopiaDbContext.SaveChangesAsync();
                
            }
           
            return RedirectToAction("Index", new { IngredID = ingredient.Ingredient_Id });
        }
    }
}
