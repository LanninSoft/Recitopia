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
    public class Ingredient_ComponentsController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public Ingredient_ComponentsController(RecitopiaDBContext recitopiaDbContext)
        {
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
        }

        [Authorize]
        // GET: Ingredient_Components
        public async Task<ActionResult> Index(int IngredID)
        {
            int customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }            

            var ingredient_Components = await _recitopiaDbContext.Ingredient_Components
                .Include(ic => ic.Components)
                .Include(i => i.Ingredients)
                .Where(ic => ic.Customer_Id == customerId).ToListAsync();
            //Filter down to id looking for
            var Ingred_nut = ingredient_Components.Where(i => i.Ingred_Id == IngredID && i.Customer_Id == customerId);

            //get Ingred name from db and pass in viewbag
            Ingredient ingredient = await _recitopiaDbContext.Ingredient.FindAsync(IngredID);

            //Assign to temp local to put on view
            ViewBag.IngredName = ingredient.Ingred_name;

            ViewBag.IngredNameID = IngredID;

            return View(Ingred_nut.ToList());
        }
        [HttpGet]
        public async Task<JsonResult> GetData(int ingredId)
        {
            int customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            //BUILD VIEW FOR ANGULARJS RENDERING
            var ingredientComponents = await _recitopiaDbContext.Ingredient_Components
                .Include(ri => ri.Ingredients)
                .Include(ri => ri.Components)
                .Where(ri => ri.Customer_Id == customerId && ri.Ingred_Id == ingredId)
                .Select(ri => new View_Angular_Ingredient_Components_Details()
                {
                    Id = ri.Id,
                    Customer_Id = ri.Customer_Id,
                    Ingred_Id = ri.Ingred_Id,
                    Ingred_name = ri.Ingredients.Ingred_name,
                    Comp_Id = ri.Comp_Id,
                    Component_Name = ri.Components.Component_Name
                })
                .ToListAsync();

            return ingredientComponents != null
                ? Json(ingredientComponents)
                : Json(new { Status = "Failure" });
        }
        // GET: Ingredient_Components/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Ingredient_Components ingredient_Components = await _recitopiaDbContext.Ingredient_Components.FindAsync(id);
            if (ingredient_Components == null)
            {
                return NotFound();
            }
            return View(ingredient_Components);
        }

        // GET: Ingredient_Components/Create
        public async Task<ActionResult> Create(int ingredID)
        {
            int customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            //get recipe name from db and pass in viewbag
            Ingredient ingredient = await _recitopiaDbContext.Ingredient.FindAsync(ingredID);

            //Assign to temp local to put on view
            ViewBag.IngredName = ingredient.Ingred_name;
            ViewBag.IngredId = ingredID;

            ViewBag.Ingred_Id = new SelectList(await _recitopiaDbContext.Ingredient.Where(m => m.Customer_Id == customerId).OrderBy(m => m.Ingred_name).ToListAsync(), "Ingredient_Id", "Ingred_name");
            ViewBag.Comp_Id = new SelectList(await _recitopiaDbContext.Components.Where(m => m.Customer_Id == customerId).OrderBy(m => m.Comp_Sort).ToListAsync(), "Comp_Id", "Component_Name");
            
            //BUILD VIEW FOR ALREADY ADDED COMPONENTS
            var ingredientComponents = await _recitopiaDbContext.Ingredient_Components
                            .Include(ri => ri.Ingredients)
                            .Include(ri => ri.Components)
                            .Where(ri => ri.Customer_Id == customerId && ri.Ingred_Id == ingredID)
                            .Select(ri => new View_All_Ingredient_Components()
                            {
                                Id = ri.Id,
                                Customer_Id = ri.Customer_Id,
                                Ingred_Id = ri.Ingred_Id,
                                Ingred_name = ri.Ingredients.Ingred_name,
                                Comp_Id = ri.Comp_Id,
                                Component_Name = ri.Components.Component_Name
                            })
                            .ToListAsync();
            
            //build list to populate previously added items
            List<string> comList = new List<string>();

            foreach (View_All_Ingredient_Components ingred in ingredientComponents)
            {
                comList.Add(ingred.Component_Name);
            }

            ViewBag.Components = comList;

            return View();

        }

        // POST: Ingredient_Components/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] Ingredient_Components ingredient_Components, int RID)
        {
            int customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (RID > 0)
            {

                ingredient_Components.Ingred_Id = RID;
                ingredient_Components.Customer_Id = customerId;
                await _recitopiaDbContext.Ingredient_Components.AddAsync(ingredient_Components);
                await _recitopiaDbContext.SaveChangesAsync();
                return RedirectToAction("Index", new { IngredID = ingredient_Components.Ingred_Id });
            }

            ViewBag.Comp_Id = new SelectList(await _recitopiaDbContext.Components.Where(m => m.Customer_Id == customerId).ToListAsync(), "Comp_Id", "Component_Name", ingredient_Components.Comp_Id);
            ViewBag.Ingred_Id = RID;
            return View(ingredient_Components);
        }

        // GET: Ingredient_Components/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }
            Ingredient_Components ingredient_Components = await _recitopiaDbContext.Ingredient_Components.FindAsync(id);
            if (ingredient_Components == null)
            {
                return NotFound();
            }
            int customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            //get name from db and pass in viewbag
            Ingredient ingredient = await _recitopiaDbContext.Ingredient.FindAsync(ingredient_Components.Ingred_Id);

            //Assign to temp local to put on view
            ViewBag.IngredName = ingredient.Ingred_name;
            ViewBag.IngredId = ingredient.Ingredient_Id;

            ViewBag.Comp_Id = new SelectList(await _recitopiaDbContext.Components.Where(m => m.Customer_Id == customerId).OrderBy(m => m.Comp_Sort).ToListAsync(), "Comp_Id", "Component_Name", ingredient_Components.Comp_Id);

            ViewBag.Ingred_Id = new SelectList(await _recitopiaDbContext.Ingredient.Where(m => m.Customer_Id == customerId).ToListAsync(), "Ingredient_Id", "Ingred_name", ingredient_Components.Ingred_Id);
            return View(ingredient_Components);
        }

        // POST: Ingredient_Components/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>  Edit([FromForm] Ingredient_Components ingredient_Components)
        {
            int customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));
            if (ModelState.IsValid)
            {
                ingredient_Components.Customer_Id = customerId;
                _recitopiaDbContext.Entry(ingredient_Components).State = EntityState.Modified;
                await _recitopiaDbContext.SaveChangesAsync();
                return RedirectToAction("Index", new { IngredID = ingredient_Components.Ingred_Id });
            }
            
            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            ViewBag.Comp_Id = new SelectList(await _recitopiaDbContext.Components.Where(m => m.Customer_Id == customerId).ToListAsync(), "Comp_Id", "Component_Name", ingredient_Components.Comp_Id);
            ViewBag.Ingred_Id = new SelectList(await _recitopiaDbContext.Ingredient.Where(m => m.Customer_Id == customerId).ToListAsync(), "Ingredient_Id", "Ingred_name", ingredient_Components.Ingred_Id);
            return View(ingredient_Components);
        }

        // GET: Ingredient_Components/Delete/5
        public async Task<ActionResult> Delete(int? id)
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

            var Ingredients_Components = await _recitopiaDbContext.Ingredient_Components.Where(m => m.Id == id).Include(r => r.Ingredients).Include(r => r.Components).ToListAsync();

            
            if (Ingredients_Components == null)
            {
                return NotFound();
            }
            //GOT TO BE A BETTER WAY THAN BUILD THE MODEL BY HAND:/
            Ingredient_Components rIngreds = new Ingredient_Components();
            Ingredient tIngred = new Ingredient();
            Components tCom = new Components();

            foreach (Ingredient_Components ingredientComponent in Ingredients_Components)
            {
                tIngred = ingredientComponent.Ingredients;
                tCom = ingredientComponent.Components;

                rIngreds.Id = ingredientComponent.Id;
                rIngreds.Ingred_Id = ingredientComponent.Ingred_Id;
                rIngreds.Ingredients = tIngred;
                rIngreds.Comp_Id = ingredientComponent.Comp_Id;
                rIngreds.Components = tCom;
            }
            return View(rIngreds);
        }

        // POST: Ingredient_Components/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Ingredient_Components ingredient_Components = await _recitopiaDbContext.Ingredient_Components.FindAsync(id);

            try
            {
                _recitopiaDbContext.Ingredient_Components.Remove(ingredient_Components);
                await _recitopiaDbContext.SaveChangesAsync();
                return RedirectToAction("Index", new { IngredID = ingredient_Components.Ingred_Id });
            }
           
            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(ingredient_Components);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "There was an issue deleting this Nutrient from this Ingredient.";
                return View(ingredient_Components);
            }

        }
    }
}
