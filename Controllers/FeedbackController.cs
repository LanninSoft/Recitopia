using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recitopia.Data;
using Recitopia.Models;
using Recitopia.Services;


namespace Recitopia.wwwroot
{
    public class FeedbackController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;
        private readonly IEmailSender _emailSender;

        public FeedbackController(RecitopiaDBContext recitopiaDbContext, IEmailSender emailSender)
        {
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        }
        [Authorize]
        // GET: Feedbacks
        public async Task<IActionResult> Index()
        {
            
            //var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            //if  (customerGuid == null || customerGuid.Trim() == "")
            //{
            //    return RedirectToAction("CustomerLogin", "Customers");
            //}

            return View(await _recitopiaDbContext.Feedback.Include(m => m.FeedbackFiles).OrderBy(m => m.Resolved).OrderByDescending(m => m.TimeStamp).ToListAsync());
        }
        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            //BUILD VIEW FOR ANGULARJS RENDERING
            var feedbacks = await _recitopiaDbContext.Feedback.Include(m => m.FeedbackFiles).OrderBy(m => m.Resolved).OrderByDescending(m => m.TimeStamp).ToListAsync();

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

            var feedback = await _recitopiaDbContext.Feedback.Include(m => m.FeedbackFiles)
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
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (ModelState.IsValid)
            {
                //TIMESTAMP, USER NAME and CUSTOMER NAME need to get input
                try
                {
                    var customerName = await _recitopiaDbContext.Customers.SingleAsync(m => m.Customer_Guid == customerGuid);
                    feedback.Customer_Name = customerName.Customer_Name;
                }
                catch (Exception)
                {
                    feedback.Customer_Name = "No Customer";
                }                    

                var userName = await _recitopiaDbContext.AppUsers.SingleAsync(m => m.UserName == User.Identity.Name);

                feedback.User_Name = userName.FullName;
                feedback.TimeStamp = DateTime.UtcNow;

                _recitopiaDbContext.Add(feedback);
                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(feedback);
        }
        public async Task<IActionResult> uploadFeedbackFile(int? id)
        {
            var feedbackRecord = await _recitopiaDbContext.Feedback.FindAsync(id);

            var feedbackFile = new FeedbackFiles()
            { 
                FeedbackSubject = feedbackRecord.Subject,
                FeedbackId = feedbackRecord.Id
            };
            
            return View(feedbackFile);
        }
        [HttpPost]
        public async Task<IActionResult> uploadFeedbackFile([FromForm] FeedbackFiles feedbackFile, IFormFile file)
        {

            if (ModelState.IsValid)
            {
                if (file.Length > 0 && file.ContentType.Contains("image") && (file.Length < (4000 * 1024)))

                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        feedbackFile.Image = ms.ToArray();
                    }

                    _recitopiaDbContext.Add(feedbackFile);
                    await _recitopiaDbContext.SaveChangesAsync();
                }
                else
                {
                    
                    
                    ViewBag.ErrorMessage = "The file is missing, is larger than 4mb, or is not an image type.  Please re-select the screenshot you wish to upload and try again.";
                    return View(feedbackFile);
                }
            
            }

            return RedirectToAction("FeedBackUserHistoryDetails", new { id = feedbackFile .FeedbackId});
        }
        public IActionResult FeedBackUser()
        {
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FeedBackUser([FromForm] Feedback feedback)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

           

            if (ModelState.IsValid)
            {

                //TIMESTAMP, USER NAME and CUSTOMER NAME need to get input
                try
                {
                    var customerName = await _recitopiaDbContext.Customers.SingleAsync(m => m.Customer_Guid == customerGuid);
                    feedback.Customer_Name = customerName.Customer_Name;
                }
                catch (Exception)
                {
                    feedback.Customer_Name = "No Customer";
                }
                var userName = await _recitopiaDbContext.AppUsers.SingleAsync(m => m.UserName == User.Identity.Name);

                feedback.User_Id = userName.Id;  
                feedback.User_Name = userName.FullName;
                feedback.TimeStamp = DateTime.UtcNow;

                _recitopiaDbContext.Add(feedback);
                await _recitopiaDbContext.SaveChangesAsync();

                //EMAIL WEBMASTER NEW FEEDBACK
                await _emailSender.SendEmailAsync("recitopia@gmail.com", "New Feedback Posted",
                       $"User - " + feedback.User_Name + "<br>Subject - " + feedback.Subject);

                ViewBag.SuccessMessage = ("Your Feedback has been successfully submitted. Thank you!");

                //return View();
                return View();
            }
            ViewBag.ErrorMessage = ("There was an issue submitting your feedback, please review your information and try again.");
            return View(feedback);
        }
        
        public async Task<IActionResult> FeedBackUserHistory()
        {
            //var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            //if  (customerGuid == null || customerGuid.Trim() == "")
            //{
            //    return RedirectToAction("CustomerLogin", "Customers");
            //}
            var currentUserId = await _recitopiaDbContext.AppUsers.SingleAsync(m => m.Email == User.Identity.Name);

            var userFeedbackResults = await _recitopiaDbContext.Feedback
                .Include(m => m.FeedbackFiles)
                .Where(m => m.User_Id == currentUserId.Id)
                .OrderBy(m => m.Resolved)
                .OrderByDescending(m => m.TimeStamp)
                .ToListAsync();

            return View(userFeedbackResults);
        }

        [HttpGet]
        public async Task<JsonResult> GetDataHistory()
        {
            //BUILD VIEW FOR ANGULARJS RENDERING
            var currentUserId = await _recitopiaDbContext.AppUsers.SingleAsync(m => m.Email == User.Identity.Name);

            var userFeedbackResults = await _recitopiaDbContext.Feedback
                .Include(m => m.FeedbackFiles)
                .Where(m => m.User_Id == currentUserId.Id)
                .OrderBy(m => m.Resolved)
                .OrderByDescending(m => m.TimeStamp)
                .ToListAsync();

            return userFeedbackResults != null
                ? Json(userFeedbackResults)
                : Json(new { Status = "Failure" });
        }
        public async Task<IActionResult> FeedBackUserHistoryDetails(int? id)
        {
            //var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            //if  (customerGuid == null || customerGuid.Trim() == "")
            //{
            //    return RedirectToAction("CustomerLogin", "Customers");
            //}

            var feedBackItem = await _recitopiaDbContext.Feedback
                .Include(m => m.FeedbackFiles)
                .Where(m => m.Id == id)
                .SingleAsync();            

            return View(feedBackItem);
        }
        public PartialViewResult FeedbackFile()
        {
            return PartialView("_FeedbackFiles");
        }
        // GET: Feedbacks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _recitopiaDbContext.Feedback
                .Include(m => m.FeedbackFiles)
                .Where(m => m.Id == id)
                .SingleAsync();

            HttpContext.Session.SetString("WasResolved", feedback.Resolved.ToString());

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
                    var previouslyResolved = HttpContext.Session.GetString("WasResolved");

                    if (feedback.Resolved == true && previouslyResolved == "False")
                    {
                        feedback.ResolvedDate = DateTime.UtcNow;
                    }
                    else if (feedback.Resolved == false && previouslyResolved == "True")                    
                    {
                        feedback.ResolvedDate = DateTime.MinValue;
                    }

                    var currentUserId = await _recitopiaDbContext.AppUsers.SingleAsync(m => m.Email == User.Identity.Name);
                   

                    feedback.UpdatedBy = currentUserId.FullName;
                    feedback.UpdatedDate = DateTime.UtcNow;



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

        public async Task<IActionResult> DeleteFile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedbackFile = await _recitopiaDbContext.FeedbackFiles
                .FirstOrDefaultAsync(m => m.ImageId == id);
            if (feedbackFile == null)
            {
                return NotFound();
            }

            return View(feedbackFile);
        }
        [HttpPost, ActionName("DeleteFile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFileConfirmed(int id)
        {
            var feedbackFile = await _recitopiaDbContext.FeedbackFiles.FindAsync(id);
            
            _recitopiaDbContext.FeedbackFiles.Remove(feedbackFile);
            await _recitopiaDbContext.SaveChangesAsync();
            return RedirectToAction("Edit", new { id = feedbackFile.FeedbackId });
            
        }

    }
}
