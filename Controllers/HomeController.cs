using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Recitopia.Data;
using Recitopia.Models;
using Microsoft.EntityFrameworkCore;

namespace Recitopia.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public HomeController(ILogger<HomeController> logger, RecitopiaDBContext recitopiaDbContext)
        {
            _logger = logger;
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
          
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.Name != null)
            {
                //Look to see if company id and name saved prior.  If so, bypass selection page and take to home
                var currentUser = await _recitopiaDbContext.AppUsers.Where(m => m.UserName == User.Identity.Name).FirstOrDefaultAsync();

                var checkLastLoginCompanyInfo = await _recitopiaDbContext.AppUsers.Where(m => m.Id == currentUser.Id).FirstOrDefaultAsync();

                if (checkLastLoginCompanyInfo.Customer_Id > 0 && checkLastLoginCompanyInfo.Customer_Name != null)
                {
                    HttpContext.Session.SetString("CurrentUserCustomerId", checkLastLoginCompanyInfo.Customer_Id.ToString());


                }
                return View();
            }
            else
            {
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
