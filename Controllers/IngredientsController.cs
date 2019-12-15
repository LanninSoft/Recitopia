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
        public JsonResult GetData()
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            //BUILD VIEW FOR ANGULARJS RENDERING
            var query =
               from t1 in _recitopiaDbContext.Ingredient.AsQueryable()
               join t2 in _recitopiaDbContext.Vendor.AsQueryable() on t1.Vendor_Id equals t2.Vendor_Id into t2g
               from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               where t1.Customer_Id == CustomerId
               orderby t1.Ingred_name
               select new View_Angular_Ingredients_Details()
               {
                   Ingredient_Id = t1.Ingredient_Id,
                   Customer_Id = t1.Customer_Id,
                   Ingred_name = t1.Ingred_name,
                   Cost_per_lb = t1.Cost_per_lb,
                   Cost = t1.Cost,
                   Package = t1.Package,
                   Vendor_Name = t2.Vendor_Name,
               };

            List<View_Angular_Ingredients_Details> ingredients = query.ToList();

            if (ingredients != null)
            {
                return Json(ingredients);
            }
            return Json(new { Status = "Failure" });
        }
        // GET: Ingredients/Details/5
        public ActionResult Details(int? id)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Ingredient ingredient = _recitopiaDbContext.Ingredient.Find(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            //ASSOCIATED NUTRITION
            var query =
               from t1 in _recitopiaDbContext.Ingredient_Nutrients.AsQueryable()
               join t2 in _recitopiaDbContext.Ingredient.AsQueryable() on t1.Ingred_Id equals t2.Ingredient_Id into t2g
               from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t3 in _recitopiaDbContext.Nutrition.AsQueryable() on t1.Nutrition_Item_Id equals t3.Nutrition_Item_Id
               where t1.Ingred_Id == id
               where t1.Customer_Id == CustomerId
               orderby t3.Nutrition_Item
               select new View_All_Ingredient_Nutrients()
               {
                   Id = t1.Id,
                   Customer_Id = t1.Customer_Id,
                   Ingred_Id = t1.Ingred_Id,
                   Nutrition_Item_Id = t1.Nutrition_Item_Id,
                   Nut_per_100_grams = t1.Nut_per_100_grams,
                   Ingred_name = t2.Ingred_name,
                   Nutrition_Item = t3.Nutrition_Item,


               };
            ViewBag.IngredientNutritions = query.ToList();

            //get recipe name from db and pass in viewbag
            Ingredient ingredient2 = _recitopiaDbContext.Ingredient.Find(ingredient.Ingredient_Id);

            //---------------------------------------------------
            //GET ALLERGENS            

            var queryC =
              from t1 in _recitopiaDbContext.Ingredient_Components.AsQueryable()
              join t2 in _recitopiaDbContext.Ingredient.AsQueryable() on t1.Ingred_Id equals t2.Ingredient_Id into t2g
              from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
              join t3 in _recitopiaDbContext.Components.AsQueryable() on t1.Comp_Id equals t3.Comp_Id
              where t1.Ingred_Id == id
              where t1.Customer_Id == CustomerId
              orderby t3.Component_Name
              select new View_All_Ingredient_Components()
              {
                  Id = t1.Id,
                  Customer_Id = t1.Customer_Id,
                  Ingred_Id = t1.Ingred_Id,
                  Comp_Id = t1.Comp_Id,
                  Ingred_name = t2.Ingred_name,
                  Component_Name = t3.Component_Name

              };


            List<View_All_Ingredient_Components> TempV = queryC.ToList();

            //build list to populate previously added items
            List<string> comList = new List<string>();

            foreach (View_All_Ingredient_Components comp in TempV)
            {
                comList.Add(comp.Component_Name);

            }

            comList = comList.Distinct().ToList();

            ViewBag.RComponents = comList;
            //----------------------------------------------------




            //Assign to temp local to put on view
            ViewBag.IngredientName = ingredient2.Ingred_name;
            ViewBag.Ing_Id = ingredient2.Ingredient_Id;

            return View(ingredient);
        }

        // GET: Ingredients/Create
        public ActionResult Create()
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            ViewBag.Vendor_Id = new SelectList(_recitopiaDbContext.Vendor.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Vendor_Name), "Vendor_Id", "Vendor_Name");
            return View();
        }

        // POST: Ingredients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] Ingredient ingredient)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (ModelState.IsValid)
            {
                ingredient.Customer_Id = CustomerId;
                _recitopiaDbContext.Ingredient.Add(ingredient);
                _recitopiaDbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Vendor_Id = new SelectList(_recitopiaDbContext.Vendor.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Vendor_Name), "Vendor_Id", "Vendor_Name", ingredient.Vendor_Id);
            return View(ingredient);
        }

        // GET: Ingredients/Edit/5
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

            Ingredient ingredient = _recitopiaDbContext.Ingredient.Find(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            //get recipe name from db and pass in viewbag
            Ingredient ingredient2 = _recitopiaDbContext.Ingredient.Find(ingredient.Ingredient_Id);

            //Assign to temp local to put on view
            ViewBag.IngredientName = ingredient2.Ingred_name;
            ViewBag.Ing_Id = ingredient2.Ingredient_Id;

            ViewBag.Vendor_Id = new SelectList(_recitopiaDbContext.Vendor.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Vendor_Name), "Vendor_Id", "Vendor_Name", ingredient.Vendor_Id);
            return View(ingredient);
        }

        // POST: Ingredients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromForm] Ingredient ingredient)
        {

            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            if (ModelState.IsValid)
            {
                ingredient.Customer_Id = CustomerId;
                _recitopiaDbContext.Entry(ingredient).State = EntityState.Modified;
                _recitopiaDbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Vendor_Id = new SelectList(_recitopiaDbContext.Vendor.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Vendor_Name), "Vendor_Id", "Vendor_Name", ingredient.Vendor_Id);
            return View(ingredient);
        }

        // GET: Ingredients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Ingredient ingredient = _recitopiaDbContext.Ingredient.Find(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            //get recipe name from db and pass in viewbag
            Ingredient ingredient2 = _recitopiaDbContext.Ingredient.Find(ingredient.Ingredient_Id);

            //Assign to temp local to put on view
            ViewBag.IngredientName = ingredient2.Ingred_name;
            ViewBag.Ing_Id = ingredient2.Ingredient_Id;

            return View(ingredient);
        }

        // POST: Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ingredient ingredient = _recitopiaDbContext.Ingredient.Find(id);

            try
            {

                _recitopiaDbContext.Ingredient.Remove(ingredient);
                _recitopiaDbContext.SaveChanges();

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
        public ActionResult CreateCopy(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }


            Ingredient ingredient = _recitopiaDbContext.Ingredient.Find(id);

            if (ingredient == null)
            {
                return NotFound();
            }
            return View(ingredient);
        }
        [HttpPost, ActionName("CreateCopy")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCopyConfirmed(int id)
        {

            Ingredient ingredient = _recitopiaDbContext.Ingredient.Find(id);
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
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

                _recitopiaDbContext.Ingredient.Add(ingredient_new);
                _recitopiaDbContext.SaveChanges();

                //COPY INGREDIENT_NUTRITION RELATIONSHIPS
                var ingredient_nutrient = _recitopiaDbContext.Ingredient_Nutrients.Where(m => m.Ingred_Id == ingredient.Ingredient_Id && m.Customer_Id == CustomerId).ToList();

                foreach (Ingredient_Nutrients thing in ingredient_nutrient)
                {
                    Ingredient_Nutrients ingredient_nutrient_new = new Ingredient_Nutrients()
                    {
                        Ingred_Id = ingredient_new.Ingredient_Id,
                        Nutrition_Item_Id = thing.Nutrition_Item_Id,
                        Nut_per_100_grams = thing.Nut_per_100_grams,
                        Customer_Id = thing.Customer_Id

                    };
                    //UPDATE DB - INSERT
                    _recitopiaDbContext.Ingredient_Nutrients.Add(ingredient_nutrient_new);
                    _recitopiaDbContext.SaveChanges();

                }
                //COPY INGREDIENT_COMPONENTS RELATIONSHIPS
                var ingredient_component = _recitopiaDbContext.Ingredient_Components.Where(m => m.Ingred_Id == ingredient.Ingredient_Id && m.Customer_Id == CustomerId).ToList();

                foreach (Ingredient_Components thing in ingredient_component)
                {
                    Ingredient_Components ingredient_component_new = new Ingredient_Components()
                    {
                        Ingred_Id = ingredient_new.Ingredient_Id,
                        Comp_Id = thing.Comp_Id,
                        Customer_Id = thing.Customer_Id

                    };
                    //UPDATE DB - INSERT
                    _recitopiaDbContext.Ingredient_Components.Add(ingredient_component_new);
                    _recitopiaDbContext.SaveChanges();

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
