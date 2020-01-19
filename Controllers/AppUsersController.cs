using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class AppUsersController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;
        private readonly UserManager<AppUser> _userManager;
        public AppUsersController(RecitopiaDBContext recitopiaDbContext, UserManager<AppUser> userManager)
        {
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [Authorize(Roles ="Administrator")]
        // GET: AppUsers
        public async Task<IActionResult> Index()
        {
            return View(await _recitopiaDbContext.AppUsers.ToListAsync());
        }
        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            var appUsers = await _recitopiaDbContext.AppUsers.ToListAsync();

            if (appUsers != null)
            {
                return Json(appUsers);
            }
            return Json(new { Status = "Failure" });
        }

        // GET: AppUsers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _recitopiaDbContext.AppUsers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // GET: AppUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AppUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                _recitopiaDbContext.Add(appUser);
                await _recitopiaDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(appUser);
        }

        // GET: AppUsers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _recitopiaDbContext.AppUsers.FindAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            IEnumerable<SelectListItem> appUserRoles = await _recitopiaDbContext.AppRoles
                .Select(c => new SelectListItem
                    {
                        Value = c.Name,
                        Text = c.Name
                    })
                .ToListAsync();

            ViewBag.UserRoles = appUserRoles;

            return View(appUser);
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [FromForm] AppUser appUser)
        {
            if (id != appUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                
                //USE LINQ TO UPDATE USER
                var appuser = await _recitopiaDbContext.AppUsers.FindAsync(id);
                try
                {
                    if (appuser != null)
                    {
                        appuser.FirstName = appUser.FirstName;
                        appuser.LastName = appUser.LastName;
                        appuser.PhoneNumber = appUser.PhoneNumber;
                        appuser.Address1 = appUser.Address1;
                        appuser.Address2 = appUser.Address2;
                        appuser.City = appUser.City;
                        appuser.State = appUser.State;
                        appuser.ZipCode = appUser.ZipCode;
                        appuser.WebUrl = appUser.WebUrl;
                        appuser.Notes = appUser.Notes;
                        appuser.Site_Role_Id = appUser.Site_Role_Id;
                        appuser.Email = appUser.Email;
                        appuser.EmailConfirmed = appUser.EmailConfirmed;
                        appuser.PhoneNumberConfirmed = appUser.PhoneNumberConfirmed;
                        appuser.TwoFactorEnabled = appUser.TwoFactorEnabled;
                        appuser.LockoutEnd = appUser.LockoutEnd;
                        appuser.LockoutEnabled = appUser.LockoutEnabled;
                        appuser.AccessFailedCount = appUser.AccessFailedCount;

                        await _recitopiaDbContext.SaveChangesAsync();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await AppUserExists(appUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //remove all other roles for user
                //add role from edit page
                var roles = await _userManager.GetRolesAsync(appuser);
                await _userManager.RemoveFromRolesAsync(appuser, roles.ToArray());

                var doIt = await _userManager.AddToRoleAsync(appuser, appuser.Site_Role_Id);
               
                return RedirectToAction(nameof(Index));
            }
            return View(appUser);
        }

        // GET: AppUsers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _recitopiaDbContext.AppUsers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // POST: AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var appUser = await _recitopiaDbContext.AppUsers.FindAsync(id);

            //NEED TO WRITE MULTIPLE CASCADING DELETES TO MAKE SURE THERE ARE NO ORPHANS
            var customerUsersRemove = await _recitopiaDbContext.Customer_Users.Where(m => m.User_Id == appUser.Id).ToListAsync();

            _recitopiaDbContext.AppUsers.Remove(appUser);
            await _recitopiaDbContext.SaveChangesAsync();

            _recitopiaDbContext.Customer_Users.RemoveRange(customerUsersRemove);
            await _recitopiaDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> AppUserExists(string id)
        {
            return await _recitopiaDbContext.AppUsers
                .AnyAsync(e => e.Id == id);
        }
    }
}
