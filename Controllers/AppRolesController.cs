using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recitopia.Models;

namespace Recitopia.Controllers
{
    public class AppRolesController : AuthorizeController
    {
        private readonly RecitopiaDBContext db;

        public AppRolesController(RecitopiaDBContext context)
        {
            db = context;
        }
        [Authorize(Roles = "Administrator")]
        // GET: AppRoles
        public async Task<IActionResult> Index()
        {
            return View(await db.Roles.ToListAsync());
        }

        [HttpGet]
        public JsonResult GetData()
        {
            int CustomerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));

            //BUILD VIEW FOR ANGULARJS RENDERING
            var query = db.AppRoles;

            List<AppRole> appRoles = query.ToList();

            if (appRoles != null)
            {
                return Json(appRoles);
            }
            return Json(new { Status = "Failure" });
        }

        // GET: AppRoles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appRole = await db.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appRole == null)
            {
                return NotFound();
            }

            return View(appRole);
        }

        // GET: AppRoles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AppRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] AppRole appRole)
        {
            if (ModelState.IsValid)
            {
                //CREATE GROUP ID
                appRole.Id = Guid.NewGuid().ToString();

                db.Add(appRole);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(appRole);
        }

        // GET: AppRoles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appRole = await db.Roles.FindAsync(id);
            if (appRole == null)
            {
                return NotFound();
            }
            return View(appRole);
        }

        // POST: AppRoles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [FromForm] AppRole appRole)
        {
            if (id != appRole.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(appRole);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppRoleExists(appRole.Id))
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
            return View(appRole);
        }

        // GET: AppRoles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appRole = await db.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appRole == null)
            {
                return NotFound();
            }

            return View(appRole);
        }

        // POST: AppRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var appRole = await db.Roles.FindAsync(id);
            db.Roles.Remove(appRole);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppRoleExists(string id)
        {
            return db.Roles.Any(e => e.Id == id);
        }
    }
}
