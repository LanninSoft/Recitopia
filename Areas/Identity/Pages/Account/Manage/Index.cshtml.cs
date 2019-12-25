using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Recitopia.Data;
using Recitopia.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Recitopia.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public IndexModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            RecitopiaDBContext recitopiaDbContext)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
        }

        [Display(Name = "User Name")]
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "First Name")]
            [Required]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            [Required]
            public string LastName { get; set; }

            [Display(Name = "Address 1")]
            public string Address1 { get; set; }

            [Display(Name = "Address 2")]
            public string Address2 { get; set; }

            [Display(Name = "City")]
            public string City { get; set; }

            [Display(Name = "State")]
            public string State { get; set; }

            [Display(Name = "Postal Code")]
            public string ZipCode { get; set; }

            [Display(Name = "Web URL")]
            public string WebUrl { get; set; }

            public int Customer_Id { get; set; }
        }

        private async Task LoadAsync(AppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var userInfo = await _userManager.FindByIdAsync(user.Id);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                Address1 = userInfo.Address1,
                Address2 = userInfo.Address2,
                City = userInfo.City,
                State = userInfo.State,
                ZipCode = userInfo.ZipCode,
                WebUrl = userInfo.WebUrl,
                Customer_Id = userInfo.Customer_Id
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            //USE LINQ TO UPDATE USER
            var appuser = await _recitopiaDbContext.AppUsers.FindAsync(user.Id);
            if (appuser != null)
            {
                appuser.FirstName = Input.FirstName;
                appuser.LastName = Input.LastName;
                appuser.PhoneNumber = Input.PhoneNumber;
                appuser.Address1 = Input.Address1;
                appuser.Address2 = Input.Address2;
                appuser.City = Input.City;
                appuser.State = Input.State;
                appuser.ZipCode = Input.ZipCode;
                appuser.WebUrl = Input.WebUrl;
                appuser.Customer_Id = Input.Customer_Id;

                await _recitopiaDbContext.SaveChangesAsync();
            }

            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "Your profile has been updated";

            return RedirectToPage();
        }
    }
}
