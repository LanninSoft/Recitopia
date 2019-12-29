using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recitopia.Data;
using Recitopia.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Recitopia.Controllers
{
    public class ComponentsController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public ComponentsController(RecitopiaDBContext recitopiaDbContext)
        {
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
        }

        [Authorize]
        // GET: Components
        public async Task<ActionResult> Index()
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (customerGuid == null)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            var components = await _recitopiaDbContext.Components
                .Where(c => c.Customer_Guid == customerGuid)
                .OrderBy(c => c.Comp_Sort)
                .ToListAsync();

            return View(components);
        }

        [HttpGet]
        public async Task<JsonResult> GetData()
        {

            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            var allergen = await _recitopiaDbContext.Components
                .Where(c => c.Customer_Guid == customerGuid)
                .OrderBy(c => c.Component_Name)
                .ToListAsync();

            return allergen != null
                ? Json(allergen)
                : Json(new { Status = "Failure" });
        }

        // GET: Components/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }

            var component = await _recitopiaDbContext.Components.FindAsync(id);

            if (component == null)
            {
                return NotFound();
            }

            return View(component);
        }

        // GET: Components/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Components/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] Components component)
        {
            if (ModelState.IsValid)
            {
                
                var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

                if (customerGuid == null)
                {
                    return RedirectToAction("CustomerLogin", "Customers");
                }

                var compFullName = component.Component_Name;

                var arr = compFullName.ToCharArray();

                arr = Array.FindAll(arr, (c => (char.IsLetterOrDigit(c)
                                                  || char.IsWhiteSpace(c)
                                                  || c == '-')));
                var compFN = new string(arr);

                var intStrLen = compFN.Length;

                component.Comp_Sort = compFN.Substring(0, intStrLen > 48 ? 48 : intStrLen);

                //---------------------------------------
                component.Customer_Guid = customerGuid;

                _recitopiaDbContext.Components.Add(component);

                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(component);
        }

        // GET: Components/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }

            var component = await _recitopiaDbContext.Components.FindAsync(id);

            if (component == null)
            {
                return NotFound();
            }

            return View(component);
        }

        // POST: Components/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([FromForm] Components component)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (ModelState.IsValid)
            {
                //Remove all extra and save 48 characters to Comp_sort field
                var compFullName = component.Component_Name;

                char[] arr = compFullName.ToCharArray();

                arr = Array.FindAll(arr, (c => (char.IsLetterOrDigit(c)
                                                  || char.IsWhiteSpace(c)
                                                  || c == '-')));
                var compFN = new string(arr);

                var intStrLen = compFN.Length;

                component.Comp_Sort = compFN.Substring(0, intStrLen > 48 ? 48 : intStrLen);

                //---------------------------------------
                component.Customer_Guid = customerGuid;
                _recitopiaDbContext.Entry(component).State = EntityState.Modified;
                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(component);
        }

        // GET: Components/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(0);
            }

            var component = await _recitopiaDbContext.Components.FindAsync(id);

            if (component == null)
            {
                return NotFound();
            }

            return View(component);
        }

        // POST: Components/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var component = await _recitopiaDbContext.Components.FindAsync(id);

            try
            {
                _recitopiaDbContext.Components.Remove(component);
                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(component);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 3";
                return View(component);
            }
        }
    }
}
