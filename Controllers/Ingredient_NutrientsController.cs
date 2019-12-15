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
        public ActionResult Index(int IngredID)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            var ingredient_Nutrients = _recitopiaDbContext.Ingredient_Nutrients.Where(m => m.Customer_Id == CustomerId).Include(i => i.Ingredients).Where(m => m.Customer_Id == CustomerId).Include(i => i.Nutrition).Where(m => m.Customer_Id == CustomerId);

            //Filter down to id looking for
            var Ingred_nut = ingredient_Nutrients.Where(i => i.Ingred_Id.Equals(IngredID) && i.Customer_Id == CustomerId);

            //Sort
            var Ingred_nut2 = Ingred_nut.OrderBy(i => i.Nutrition.OrderOnNutrientPanel);

            //get Ingred name from db and pass in viewbag
            Ingredient ingredient = _recitopiaDbContext.Ingredient.Find(IngredID);

            //Assign to temp local to put on view
            ViewBag.IngredName = ingredient.Ingred_name;

            ViewBag.IngredNameID = IngredID;

            return View(Ingred_nut2.ToList());
        }

        // GET: Ingredient_Nutrients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Ingredient_Nutrients ingredient_Nutrients = _recitopiaDbContext.Ingredient_Nutrients.Find(id);
            if (ingredient_Nutrients == null)
            {
                return NotFound();
            }
            return View(ingredient_Nutrients);
        }

        // GET: Ingredient_Nutrients/Create
        public ActionResult Create(int IngredID)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            //get recipe name from db and pass in viewbag
            Ingredient ingredient = _recitopiaDbContext.Ingredient.Find(IngredID);

            //Assign to temp local to put on view
            ViewBag.IngredName = ingredient.Ingred_name;
            ViewBag.IngredId = IngredID;

            ViewBag.Ingred_Id = new SelectList(_recitopiaDbContext.Ingredient.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Ingred_name), "Ingredient_Id", "Ingred_name");
            ViewBag.Nutrition_Item_Id = new SelectList(_recitopiaDbContext.Nutrition.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Nutrition_Item), "Nutrition_Item_Id", "Nutrition_Item");


            //BUILD VIEW FOR ALREADY ADDED NUTRIENTS
            var query =
               from t1 in _recitopiaDbContext.Ingredient_Nutrients.AsQueryable()
               join t2 in _recitopiaDbContext.Ingredient.AsQueryable() on t1.Ingred_Id equals t2.Ingredient_Id into t2g
               from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t3 in _recitopiaDbContext.Nutrition.AsQueryable() on t1.Nutrition_Item_Id equals t3.Nutrition_Item_Id
               where t1.Ingred_Id == IngredID
               where t1.Customer_Id == CustomerId
               orderby t3.Nutrition_Item
               select new View_All_Ingredient_Nutrients()
               {
                   Id = t1.Id,
                   Customer_Id = t1.Customer_Id,
                   Ingred_Id = t1.Ingred_Id,
                   Ingred_name = t2.Ingred_name,
                   Nutrition_Item_Id = t1.Nutrition_Item_Id,
                   Nutrition_Item = t3.Nutrition_Item,
                   Nut_per_100_grams = t1.Nut_per_100_grams,
                   

               };
            //Get View and filter and sort            
            var TempV = query.ToList();

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
        public ActionResult Create([FromForm] Ingredient_Nutrients ingredient_Nutrients, int RID)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (RID > 0)
            {
                ViewBag.Ingred_Id = RID;
                ingredient_Nutrients.Ingred_Id = RID;
                ingredient_Nutrients.Customer_Id = CustomerId;
                _recitopiaDbContext.Ingredient_Nutrients.Add(ingredient_Nutrients);

                _recitopiaDbContext.SaveChanges();
                return RedirectToAction("Index", new { IngredID = ingredient_Nutrients.Ingred_Id });
            }

            ViewBag.Ingred_Id = RID;
            ViewBag.Nutrition_Item_Id = new SelectList(_recitopiaDbContext.Nutrition.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Nutrition_Item), "Nutrition_Item_Id", "Nutrition_Item", ingredient_Nutrients.Nutrition_Item_Id);
            return View(ingredient_Nutrients);
        }

        // GET: Ingredient_Nutrients/Edit/5
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

            //get ingred id
            Ingredient_Nutrients ingredient_nutrients = _recitopiaDbContext.Ingredient_Nutrients.Find(id);
            if (ingredient_nutrients == null)
            {
                return NotFound();
            }
            //get name from db and pass in viewbag
            Ingredient ingredient = _recitopiaDbContext.Ingredient.Find(ingredient_nutrients.Ingred_Id);

            //Assign to temp local to put on view
            ViewBag.IngredName = ingredient.Ingred_name;
            ViewBag.IngredId = ingredient.Ingredient_Id;



            ViewBag.Ingred_Id = new SelectList(_recitopiaDbContext.Ingredient.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Ingred_name), "Ingredient_Id", "Ingred_name", ingredient_nutrients.Ingred_Id);
            ViewBag.Nutrition_Item_Id = new SelectList(_recitopiaDbContext.Nutrition.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Nutrition_Item), "Nutrition_Item_Id", "Nutrition_Item", ingredient_nutrients.Nutrition_Item_Id);
            return View(ingredient_nutrients);
        }

        // POST: Ingredient_Nutrients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromForm] Ingredient_Nutrients ingredient_Nutrients)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (ModelState.IsValid)
            {
                ingredient_Nutrients.Customer_Id = CustomerId;
                _recitopiaDbContext.Entry(ingredient_Nutrients).State = EntityState.Modified;
                _recitopiaDbContext.SaveChanges();
                return RedirectToAction("Index", new { IngredID = ingredient_Nutrients.Ingred_Id });
            }
            ViewBag.Ingred_Id = new SelectList(_recitopiaDbContext.Ingredient.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Ingred_name), "Ingredient_Id", "Ingred_name", ingredient_Nutrients.Ingred_Id);
            ViewBag.Nutrition_Item_Id = new SelectList(_recitopiaDbContext.Nutrition.Where(m => m.Customer_Id == CustomerId).OrderBy(m => m.Nutrition_Item), "Nutrition_Item_Id", "Nutrition_Item", ingredient_Nutrients.Nutrition_Item_Id);
            return View(ingredient_Nutrients);
        }
        [HttpGet]
        public JsonResult GetData(int ingredId)
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            //BUILD VIEW FOR ANGULARJS RENDERING
            var query =
               from t1 in _recitopiaDbContext.Ingredient_Nutrients.AsQueryable()
               join t2 in _recitopiaDbContext.Ingredient.AsQueryable() on t1.Ingred_Id equals t2.Ingredient_Id into t2g
               from t2 in t2g.DefaultIfEmpty() //DefaultIfEmpty is used for left joins
               join t3 in _recitopiaDbContext.Nutrition.AsQueryable() on t1.Nutrition_Item_Id equals t3.Nutrition_Item_Id
               where t1.Ingred_Id == ingredId
               where t1.Customer_Id == CustomerId
               orderby t3.Nutrition_Item
               select new View_Angular_Ingredient_Nutrients_Details()
               {
                   Id = t1.Id,
                   Customer_Id = t1.Customer_Id,
                   Ingred_Id = t1.Ingred_Id,
                   Ingred_name = t2.Ingred_name,
                   Nutrition_Item_Id = t1.Nutrition_Item_Id,
                   Nutrition_Item = t3.Nutrition_Item,
                   Nut_per_100_grams = t1.Nut_per_100_grams,
                   OrderOnNutrientPanel = t3.OrderOnNutrientPanel,
                   ShowOnNutrientPanel = t3.ShowOnNutrientPanel,

               };


            List<View_Angular_Ingredient_Nutrients_Details> ingredientcomponent = query.ToList();

            if (ingredientcomponent != null)
            {
                return Json(ingredientcomponent);
            }
            return Json(new { Status = "Failure" });
        }
        //UPDATE FROM ANGULAR

        [HttpPost]
        public JsonResult UpdateFromAngularController([FromBody]IEnumerable<View_Angular_Ingredient_Nutrients_Details> jsonstring)
        {

            foreach (View_Angular_Ingredient_Nutrients_Details thing in jsonstring)
            {
                if (thing.Nut_per_100_grams > 0)
                {

                    //DIRECT LINQ TO DB REPLACING STORED PROCEDURE
                    Ingredient_Nutrients ingredient_nutrients = _recitopiaDbContext.Ingredient_Nutrients.Where(i => i.Id == thing.Id).Single();

                    ingredient_nutrients.Nut_per_100_grams = thing.Nut_per_100_grams;

                    _recitopiaDbContext.SaveChanges();

                }

            }


            return Json(jsonstring);

        }
        public class GetAll
        {
            public string FirstName { get; set; }
            public string SecondName { get; set; }
            public int Ingred_Id { get; set; }
            public int Nutrition_Item_Id { get; set; }

        }

        // GET: Ingredient_Nutrients/Delete/5
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

            var Ingredients_Nutrients = _recitopiaDbContext.Ingredient_Nutrients.Where(m => m.Id == id).Include(r => r.Ingredients).Where(m => m.Customer_Id == CustomerId).Include(r => r.Nutrition).Where(m => m.Customer_Id == CustomerId);

            // var recipe_Ingredients = recipe_Ingredients_Temp.Where(m => m.Id == id);

            if (Ingredients_Nutrients == null)
            {
                return NotFound();
            }
            //GOT TO BE A BETTER WAY THAN BUILD THE MODEL BY HAND:/
            Ingredient_Nutrients rIngreds = new Ingredient_Nutrients();
            Ingredient tIngred = new Ingredient();
            Nutrition tNut = new Nutrition();

            foreach (Ingredient_Nutrients thing in Ingredients_Nutrients)
            {
                tIngred = thing.Ingredients;
                tNut = thing.Nutrition;

                rIngreds.Id = thing.Id;
                rIngreds.Ingred_Id = thing.Ingred_Id;
                rIngreds.Ingredients = tIngred;
                rIngreds.Nutrition_Item_Id = thing.Nutrition_Item_Id;
                rIngreds.Nutrition = tNut;
                rIngreds.Customer_Id = thing.Customer_Id;

            }

            //Ingredient_Nutrients ingredient_Nutrients = db.Ingredient_Nutrients.Find(id);
            //if (ingredient_Nutrients == null)
            //{
            //    return NotFound();
            //}

            return View(rIngreds);
        }

        // POST: Ingredient_Nutrients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ingredient_Nutrients ingredient_Nutrients = _recitopiaDbContext.Ingredient_Nutrients.Find(id);


            try
            {
                _recitopiaDbContext.Ingredient_Nutrients.Remove(ingredient_Nutrients);
                _recitopiaDbContext.SaveChanges();
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
        public ActionResult PrimeIngredNutrient(int? id)
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
        [HttpPost, ActionName("PrimeIngredNutrient")]
        [ValidateAntiForgeryToken]
        public ActionResult PrimeIngredNutrientConfirmed(int id)
        {

            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (CustomerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            Ingredient ingredient = _recitopiaDbContext.Ingredient.Find(id);
           
            var nutrients = _recitopiaDbContext.Nutrition.Where(m => m.ShowOnNutrientPanel == true && m.Customer_Id == CustomerId).ToList();

            foreach (Nutrition thing in nutrients)
            {
                Ingredient_Nutrients ingredient_nutrients = new Ingredient_Nutrients() {
                    Ingred_Id = ingredient.Ingredient_Id,
                    Nutrition_Item_Id = thing.Nutrition_Item_Id,
                    Nut_per_100_grams = 0,
                    Customer_Id = ingredient.Customer_Id

                };
                //UPDATE DB - INSERT
                _recitopiaDbContext.Ingredient_Nutrients.Add(ingredient_nutrients);
                _recitopiaDbContext.SaveChanges();
                
            }

           
            return RedirectToAction("Index", new { IngredID = ingredient.Ingredient_Id });



        }
    }
}
