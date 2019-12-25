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

namespace Recitopia.wwwroot
{
    public class FeedbackController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public FeedbackController(RecitopiaDBContext recitopiaDbContext)
        {
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
        }
        [Authorize]
        // GET: Feedbacks
        public async Task<IActionResult> Index()
        {
            var customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));

            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            return View(await _recitopiaDbContext.Feedback.OrderBy(m => m.Resolved).OrderByDescending(m => m.TimeStamp).ToListAsync());
        }
        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            //BUILD VIEW FOR ANGULARJS RENDERING
            var feedbacks = await _recitopiaDbContext.Feedback.OrderBy(m => m.Resolved).OrderByDescending(m => m.TimeStamp).ToListAsync();

            return feedbacks != null
                ? Json(feedbacks)
                : Json(new { Status = "Failure" });
        }
        // GET: Feedbacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _recitopiaDbContext.Feedback
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // GET: Feedbacks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Feedbacks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Feedback feedback)
        {
            var customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));

            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (ModelState.IsValid)
            {

                //TIMESTAMP, USER NAME and CUSTOMER NAME need to get input
                var customerName = await _recitopiaDbContext.Customers.SingleAsync(m => m.Customer_Id == customerId);
                var userName = await _recitopiaDbContext.AppUsers.SingleAsync(m => m.UserName == User.Identity.Name);
                
                feedback.Customer_Name = customerName.Customer_Name;
                feedback.User_Name = userName.FullName;
                feedback.TimeStamp = DateTime.UtcNow;

                _recitopiaDbContext.Add(feedback);
                await _recitopiaDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(feedback);
        }
        public IActionResult FeedBackUser()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FeedBackUser([FromForm] Feedback feedback)
        {
            var customerId = GetUserCustomerId(HttpContext.Session.GetString("CurrentUserCustomerId"));

            if (customerId == 0)
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (ModelState.IsValid)
            {

                //TIMESTAMP, USER NAME and CUSTOMER NAME need to get input
                var customerName = await _recitopiaDbContext.Customers.SingleAsync(m => m.Customer_Id == customerId);
                var userName = await _recitopiaDbContext.AppUsers.SingleAsync(m => m.UserName == User.Identity.Name);

                feedback.Customer_Name = customerName.Customer_Name;
                feedback.User_Name = userName.FullName;
                feedback.TimeStamp = DateTime.UtcNow;

                _recitopiaDbContext.Add(feedback);
                await _recitopiaDbContext.SaveChangesAsync();

                ViewBag.SuccessMessage = ("Your Feedback has been successfully submitted. Thank you!");

                //return View();
                return View();
            }
            ViewBag.ErrorMessage = ("There was an issue submitting your feedback, please review your information and try again.");
            return View(feedback);
        }
        // GET: Feedbacks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _recitopiaDbContext.Feedback.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Feedback feedback)
        {
            if (id != feedback.Id)
            {
                return NotFound();
            }
            //NOT carrying username and customername back
            if (ModelState.IsValid)
            {
                try
                {
                    _recitopiaDbContext.Update(feedback);
                    await _recitopiaDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.Id))
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
            return View(feedback);
        }

        // GET: Feedbacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _recitopiaDbContext.Feedback
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await _recitopiaDbContext.Feedback.FindAsync(id);
            _recitopiaDbContext.Feedback.Remove(feedback);
            await _recitopiaDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool FeedbackExists(int id)
        {
            return _recitopiaDbContext.Feedback.Any(e => e.Id == id);
        }
    }
}
