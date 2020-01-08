using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using Recitopia.Data;
using Recitopia.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Recitopia.Controllers
{
    public class Recipe_PackagingController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public Recipe_PackagingController(RecitopiaDBContext recitopiaDbContext)
        {
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
        }
        [Authorize]
        // GET: Recipe_Packaging
        public async Task<IActionResult> Index(int recipeId)
        {

            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            var packaging = await _recitopiaDbContext.Recipe.FindAsync(recipeId);

            if (packaging == null)
            {
                return NotFound();
            }

            var recipePackaging =
                 await (from rp in _recitopiaDbContext.Recipe_Packaging
                        join r in _recitopiaDbContext.Recipe
                        on rp.Recipe_Id equals r.Recipe_Id
                        join p in _recitopiaDbContext.Packaging
                        on rp.Package_Id equals p.Package_Id
                        where r.Recipe_Id == recipeId && r.Customer_Guid == customerGuid
                        orderby p.Package_Name
                        select new Recipe_Packaging()
                        {
                            Id = rp.Id,
                            Customer_Guid = rp.Customer_Guid,
                            Recipe_Id = rp.Recipe_Id,
                            Package_Id = rp.Package_Id,
                            Amount = rp.Amount,
                            Packaging = p,
                            Recipe = r
                        })
                .ToListAsync();
  

            ViewBag.RecipeName = packaging.Recipe_Name;
            ViewBag.RecipeNameID = recipeId;
            

            return View(recipePackaging);

        }
        [HttpGet]
        public async Task<JsonResult> GetData(int recipeId)
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            
            var recipeIngredientDetails =
                 await (from rp in _recitopiaDbContext.Recipe_Packaging
                        join r in _recitopiaDbContext.Recipe
                        on rp.Recipe_Id equals r.Recipe_Id
                        join p in _recitopiaDbContext.Packaging
                        on rp.Package_Id equals p.Package_Id
                        where r.Recipe_Id == recipeId && r.Customer_Guid == customerGuid
                        orderby p.Package_Name
                        select new View_All_Recipe_Packaging()
                        {
                            Id = rp.Id,
                            Customer_Guid = rp.Customer_Guid,
                            Recipe_Id = rp.Recipe_Id,
                            Package_Id = rp.Package_Id,
                            Amount = rp.Amount,
                            Package_Name = p.Package_Name,
                            Recipe_Name = r.Recipe_Name
                        })
                .ToListAsync();




            return recipeIngredientDetails != null
                ? Json(recipeIngredientDetails)
                : Json(new { Status = "Failure" });
        }
        // GET: Recipe_Packaging/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe_Packaging = await _recitopiaDbContext.Recipe_Packaging
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe_Packaging == null)
            {
                return NotFound();
            }

            return View(recipe_Packaging);
        }

        // GET: Recipe_Packaging/Create
        public async Task<IActionResult> Create(int recipeID)
        {

            //get recipe name from db and pass in viewbag
            Recipe recipe = await _recitopiaDbContext.Recipe.FindAsync(recipeID);

            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            //Assign to temp local to put on view
            ViewBag.RecipeName = recipe.Recipe_Name;
            ViewBag.Rec_Id = recipeID;

            ViewBag.Package_Id = new SelectList(await _recitopiaDbContext.Packaging.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Package_Name).ToListAsync(), "Package_Id", "Package_Name");
            ViewBag.Recipe_Id = new SelectList(await _recitopiaDbContext.Recipe.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Recipe_Name).ToListAsync(), "Recipe_Id", "Recipe_Name");
            //REFACTOR - get rid of model and populate directly
            //CREATE LIST OF PREVIOUS ADDED INGREDIENTS
            var recipePackaging =
                 await (from rp in _recitopiaDbContext.Recipe_Packaging
                        join r in _recitopiaDbContext.Recipe
                        on rp.Recipe_Id equals r.Recipe_Id
                        join p in _recitopiaDbContext.Packaging
                        on rp.Package_Id equals p.Package_Id
                        where r.Recipe_Id == recipeID && r.Customer_Guid == customerGuid
                        orderby p.Package_Name
                        select new View_All_Recipe_Packaging()
                        {
                            Id = rp.Id,
                            Customer_Guid = rp.Customer_Guid,
                            Recipe_Id = rp.Recipe_Id,
                            Package_Id = rp.Package_Id,
                            Amount = rp.Amount,
                            Package_Name = p.Package_Name,
                            Recipe_Name = r.Recipe_Name
                        })
                .ToListAsync();


            //build list to populate previously added items
            List<string> ingList = new List<string>();

            foreach (View_All_Recipe_Packaging ingred in recipePackaging)
            {
                if (ingred.Amount.ToString() != null)
                {
                    ingList.Add(ingred.Package_Name + "/" + ingred.Amount.ToString() + "g");
                }
                else
                {
                    ingList.Add(ingred.Package_Name + "/" + 0.ToString() + "g");
                }
            }

            ViewBag.Packaging = ingList;



            return View();
        }

        // POST: Recipe_Packaging/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Recipe_Packaging recipe_Packaging, int RID)
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");
            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }
            if (RID > 0)
            {

                recipe_Packaging.Recipe_Id = RID;
                recipe_Packaging.Customer_Guid = customerGuid;
                await _recitopiaDbContext.Recipe_Packaging.AddAsync(recipe_Packaging);
                await _recitopiaDbContext.SaveChangesAsync();

                //DIRECT LINQ TO DB REPLACING STORED PROCEDURE
                Recipe recipeFind = await _recitopiaDbContext.Recipe.SingleAsync(i => i.Recipe_Id == recipe_Packaging.Recipe_Id);

                recipeFind.LastModified = DateTime.UtcNow;

                await _recitopiaDbContext.SaveChangesAsync();

                //REFACTOR - get rid of model and populate directly
                //CREATE LIST OF PREVIOUS ADDED INGREDIENTS
                var recipePackaging =
                 await (from rp in _recitopiaDbContext.Recipe_Packaging
                        join r in _recitopiaDbContext.Recipe
                        on rp.Recipe_Id equals r.Recipe_Id
                        join p in _recitopiaDbContext.Packaging
                        on rp.Package_Id equals p.Package_Id
                        where r.Recipe_Id == RID && r.Customer_Guid == customerGuid
                        orderby p.Package_Name
                        select new View_All_Recipe_Packaging()
                        {
                            Id = rp.Id,
                            Customer_Guid = rp.Customer_Guid,
                            Recipe_Id = rp.Recipe_Id,
                            Package_Id = rp.Package_Id,
                            Amount = rp.Amount,
                            Package_Name = p.Package_Name,
                            Recipe_Name = r.Recipe_Name
                        })
                .ToListAsync();

                //build list to populate previously added items
                List<string> ingList = new List<string>();

                foreach (View_All_Recipe_Packaging ingred in recipePackaging)
                {
                    if (ingred.Amount.ToString() != null)
                    {
                        ingList.Add(ingred.Package_Name + "/" + ingred.Amount.ToString() + "g");
                    }
                    else
                    {
                        ingList.Add(ingred.Package_Name + "/" + 0.ToString() + "g");
                    }
                }

                ViewBag.Packaging = ingList;



                return RedirectToAction("Index", new { recipeID = recipe_Packaging.Recipe_Id });
            }

            ViewBag.Packaging_Id = new SelectList(await _recitopiaDbContext.Packaging.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Package_Name).ToListAsync(), "Package_Id", "Package_Name");
            ViewBag.Recipe_Id = new SelectList(await _recitopiaDbContext.Recipe.Where(m => m.Customer_Guid == customerGuid).OrderBy(m => m.Recipe_Name).ToListAsync(), "Recipe_Id", "Recipe_Name");


            return View(recipe_Packaging);
           
        }

        // GET: Recipe_Packaging/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe_Packaging = await _recitopiaDbContext.Recipe_Packaging.FindAsync(id);
            if (recipe_Packaging == null)
            {
                return NotFound();
            }
            return View(recipe_Packaging);
        }

        // POST: Recipe_Packaging/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Recipe_Packaging recipe_Packaging)
        {
            if (id != recipe_Packaging.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _recitopiaDbContext.Update(recipe_Packaging);
                    await _recitopiaDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Recipe_PackagingExists(recipe_Packaging.Id))
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
            return View(recipe_Packaging);
        }

        // GET: Recipe_Packaging/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            var recipePackaging =
                 await (from rp in _recitopiaDbContext.Recipe_Packaging
                        join r in _recitopiaDbContext.Recipe
                        on rp.Recipe_Id equals r.Recipe_Id
                        join p in _recitopiaDbContext.Packaging
                        on rp.Package_Id equals p.Package_Id
                        where rp.Id == id
                        select new Recipe_Packaging()
                        {
                            Id = rp.Id,
                            Customer_Guid = rp.Customer_Guid,
                            Recipe_Id = rp.Recipe_Id,
                            Package_Id = rp.Package_Id,
                            Amount = rp.Amount,
                            Packaging = p,
                            Recipe = r
                        })
                .FirstOrDefaultAsync();

            

            if (recipePackaging == null)
            {
                return NotFound();
            }

            return View(recipePackaging);
        }

        // POST: Recipe_Packaging/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipePackaging =
                 await (from rp in _recitopiaDbContext.Recipe_Packaging
                        join r in _recitopiaDbContext.Recipe
                        on rp.Recipe_Id equals r.Recipe_Id
                        join p in _recitopiaDbContext.Packaging
                        on rp.Package_Id equals p.Package_Id
                        where rp.Id == id
                        select new Recipe_Packaging()
                        {
                            Id = rp.Id,
                            Customer_Guid = rp.Customer_Guid,
                            Recipe_Id = rp.Recipe_Id,
                            Package_Id = rp.Package_Id,
                            Amount = rp.Amount,
                            Packaging = p,
                            Recipe = r
                        })
                .FirstOrDefaultAsync();

            try { 
                _recitopiaDbContext.Recipe_Packaging.Remove(recipePackaging);
                 await _recitopiaDbContext.SaveChangesAsync();
                return RedirectToAction("Index", new { recipeId = recipePackaging.Recipe_Id });
            }
            
            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(recipePackaging);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "This was a problem deleting this Ingredient from this Recipe.";
                return View(recipePackaging);
            }
            
        }

        private bool Recipe_PackagingExists(int id)
        {
            return _recitopiaDbContext.Recipe_Packaging.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateFromAngularController([FromBody]List<View_All_Recipe_Packaging> recipePackagingDetails)
        {
            var recipeId = recipePackagingDetails.First().Recipe_Id;

            foreach (View_All_Recipe_Packaging detail in recipePackagingDetails)
            {

                var recipePackage = await _recitopiaDbContext.Recipe_Packaging
                        .SingleAsync(i => i.Id == detail.Id);

                if (detail.Amount >= 0)
                {
                    recipePackage.Amount = detail.Amount;

                }
                else
                {
                    recipePackage.Amount = 0;

                }
                await _recitopiaDbContext.SaveChangesAsync();
            }

            var recipe = await _recitopiaDbContext.Recipe.SingleAsync(i => i.Recipe_Id == recipeId);
            recipe.LastModified = DateTime.UtcNow;

            await _recitopiaDbContext.SaveChangesAsync();

            return Json(recipePackagingDetails);
        }
    }
}
