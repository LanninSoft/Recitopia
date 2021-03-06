﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Recitopia.Data;
using Recitopia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Recitopia.Controllers
{
    public class CustomersController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<AppUser> _userManager;

        public CustomersController(RecitopiaDBContext recitopiaDbContext, IEmailSender emailSender, UserManager<AppUser> userManager)
        {
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [Authorize(Roles = "Administrator")]
        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View(await _recitopiaDbContext.Customers.ToListAsync());
        }
        public async Task<IActionResult> SearchCustomerGroup()
        {
            return View(await _recitopiaDbContext.Customers.ToListAsync());
        }
        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            //BUILD VIEW FOR ANGULARJS RENDERING
            var customers = await _recitopiaDbContext.Customers.ToListAsync();

            return customers != null
                ? Json(customers)
                : Json(new { Status = "Failure" });
        }

        public async Task<IActionResult> CustomerLogin(bool? clickLinktoChange)
        {
            var customerCount = 0;
            //RETURN LIST OF CUSTOMERS THAT A MEMBER OF
           
            if (User.Identity.Name != null)
            {

                var currentUser = await _recitopiaDbContext.AppUsers
                .SingleAsync(m => m.UserName == User.Identity.Name);

                var customerIds = await _recitopiaDbContext.Customer_Users
                    .Where(cu => cu.User_Id == currentUser.Id)
                    .Select(cu => cu.Customer_Guid)
                    .ToListAsync();
            
                customerCount = customerIds.Count();

                if (customerCount > 1 && clickLinktoChange != true)
                {

                    //Look to see if company id and name saved prior.  If so, bypass selection page and take to home
                    var checkLastLoginCompanyInfo = await _recitopiaDbContext.AppUsers.SingleAsync(m => m.Id == currentUser.Id);

                    if (checkLastLoginCompanyInfo.Customer_Guid != null && checkLastLoginCompanyInfo.Customer_Name != null)
                    {
                         var customerGuid = await _recitopiaDbContext.Customers.SingleAsync(m => m.Customer_Guid == checkLastLoginCompanyInfo.Customer_Guid);

                        HttpContext.Session.SetString("CurrentUserCustomerGuid", customerGuid.Customer_Guid);
                        
                        //INSERT LOGIN RECORD
                        var auditEntry = new AuditLog()
                        {
                            UserId = currentUser.Id,
                            EventDate = DateTime.UtcNow,
                            Event = "Login",
                            WhatChanged = "Logged in",
                            CustomerGuid = checkLastLoginCompanyInfo.Customer_Guid
                        };
                        _recitopiaDbContext.Add(auditEntry);
                        await _recitopiaDbContext.SaveChangesAsync();

                        return LocalRedirect("~/Home/Index");
                    }
                    else
                    {
                        //Provide selection view
                        //create list and populate with Customer name and Id
                        List<IList<string>> custList = new List<IList<string>>();

                        foreach (var customerId in customerIds)
                        {
                            var tempResults = await _recitopiaDbContext.Customers.SingleAsync(m => m.Customer_Guid == customerId);

                            custList.Add(new List<string> { tempResults.Customer_Name, tempResults.Customer_Guid });
                        }
                        ViewBag.UserCustomers = custList;

                        return View();
                    }
                }
                else if (customerCount >= 1 && clickLinktoChange == true)
                {
                    //Provide selection view
                    //create list and populate with Customer name and Id
                    List<IList<string>> custList = new List<IList<string>>();

                    foreach (var customerId in customerIds)
                    {
                        var tempResults = await _recitopiaDbContext.Customers.SingleAsync(m => m.Customer_Guid == customerId);

                        custList.Add(new List<string> { tempResults.Customer_Name, tempResults.Customer_Guid });
                    }
                    ViewBag.UserCustomers = custList;

                    return View();
                }
                else if (customerCount == 1)
                {
                    //take them to home page
                    var customerCId = await _recitopiaDbContext.Customer_Users.SingleAsync(m => m.User_Id == currentUser.Id);

                    return RedirectToAction("CustomerLoginGo", new { id = customerCId.Customer_Guid });
                }
                else
                {
                    ViewBag.UserCustomers = null;

                    return View();
                }

            }

            return View();

        }

        public async Task<IActionResult> CustomerLoginGo(string id)
        {
            //save customerguid to appuser field to carry
            var currentUser = await _recitopiaDbContext.AppUsers.Where(m => m.UserName ==  User.Identity.Name).FirstAsync();
            try
            {
                var getCustomerName = await _recitopiaDbContext.Customers.Where(m => m.Customer_Guid == id).FirstAsync();

                currentUser.Customer_Guid = id;
            
                currentUser.Customer_Name = getCustomerName.Customer_Name;

                HttpContext.Session.SetString("CurrentUserCustomerGuid", id);

                await _recitopiaDbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                HttpContext.Session.SetString("CurrentUserCustomerGuid", "");
            }     

            //INSERT LOGIN RECORD
            var auditEntry = new AuditLog()
            {
                UserId = currentUser.Id,
                EventDate = DateTime.UtcNow,
                Event = "Login",
                WhatChanged = "Logged in",
                CustomerGuid = id
            };
            _recitopiaDbContext.Add(auditEntry);

            await _recitopiaDbContext.SaveChangesAsync();

            return LocalRedirect("~/Home/Index");
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _recitopiaDbContext.Customers
                .FirstOrDefaultAsync(m => m.Customer_Id == id);

            if (customers == null)
            {
                return NotFound();
            }

            return View(customers);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Customers customers)
        {
            if (ModelState.IsValid)
            {
                customers.Customer_Guid = Guid.NewGuid().ToString();
                _recitopiaDbContext.Add(customers);

                await _recitopiaDbContext.SaveChangesAsync();

                //PRIME VENDOR, SERVING SIZE AND CATEGORY WITH NA VALUE SO RECIPES/INGREDIENTS CAN BE CREATED WITH THESE DEPENDENCIES RIGHT AWAY
                //VENDOR
                var vendorPrime = new Vendor()
                {
                    Vendor_Name = "Example Vendor",
                    Notes = "Default Value added as example",
                    Phone = "000-000-0000",
                    Email = "ExampleVendorEmail@gmail.com",
                    Address1 = "1234 Vendor Lane",
                    City = "Recipeville",
                    State = "MN",
                    Zip = 12345,
                    Web_URL = "http://www.examplevendorURL.com",
                    Customer_Guid = customers.Customer_Guid
                };
                
                _recitopiaDbContext.Vendor.Add(vendorPrime);

                await _recitopiaDbContext.SaveChangesAsync();

                //SERVING SIZE
                var ssPrime = new Serving_Sizes()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Serving_Size = 1,
                    Notes = "Default Value added as example"
                };

                _recitopiaDbContext.Serving_Sizes.Add(ssPrime);

                await _recitopiaDbContext.SaveChangesAsync();

                //CATEGORY
                var categoryPrime = new Meal_Category()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Category_Name = "Snack",
                    Notes = "Default Value added as example"
                };

                _recitopiaDbContext.Meal_Category.Add(categoryPrime);

                await _recitopiaDbContext.SaveChangesAsync();

                //----------------------------------------------------------------------------------------------------
                //----------START ADD DEFAULT NUTRIENTS
                //----------------------------------------------------------------------------------------------------
                //ADD DEFAULT NUTRITION - manually put these in for now

                //CALORIES
                var nutritionCal = new Nutrition()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Nutrition_Item = "Calories",
                    DV = null,
                    Measurement = null,
                    OrderOnNutrientPanel = 1,
                    ShowOnNutrientPanel = true
                };
                _recitopiaDbContext.Nutrition.Add(nutritionCal);
                //await _recitopiaDbContext.SaveChangesAsync();

                //TOTAL FAT
                var nutritionTF = new Nutrition()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Nutrition_Item = "Total Fat",
                    DV = 78,
                    Measurement = "g",
                    OrderOnNutrientPanel = 2,
                    ShowOnNutrientPanel = true
                };
                _recitopiaDbContext.Nutrition.Add(nutritionTF);

                //Saturated Fat
                var saturatedFat = new Nutrition()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Nutrition_Item = "Saturated Fat",
                    DV = 20,
                    Measurement = "g",
                    OrderOnNutrientPanel = 3,
                    ShowOnNutrientPanel = true
                };
                _recitopiaDbContext.Nutrition.Add(saturatedFat);

                //Trans Fat
                var transFat = new Nutrition()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Nutrition_Item = "Trans Fat",
                    DV = 0,
                    Measurement = "g",
                    OrderOnNutrientPanel = 4,
                    ShowOnNutrientPanel = true
                };
                _recitopiaDbContext.Nutrition.Add(transFat);

                //Cholesterol
                var cholesterol = new Nutrition()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Nutrition_Item = "Cholesterol",
                    DV = 300,
                    Measurement = "mg",
                    OrderOnNutrientPanel = 5,
                    ShowOnNutrientPanel = true
                };
                _recitopiaDbContext.Nutrition.Add(cholesterol);

                //Sodium
                var sodium = new Nutrition()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Nutrition_Item = "Sodium",
                    DV = 2300,
                    Measurement = "mg",
                    OrderOnNutrientPanel = 6,
                    ShowOnNutrientPanel = true
                };
                _recitopiaDbContext.Nutrition.Add(sodium);

                //Total Carbohydrate
                var totalCarbohydrate = new Nutrition()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Nutrition_Item = "Total Carbohydrate",
                    DV = 275,
                    Measurement = "g",
                    OrderOnNutrientPanel = 7,
                    ShowOnNutrientPanel = true
                };
                _recitopiaDbContext.Nutrition.Add(totalCarbohydrate);

                //Dietary Fiber
                var dietaryFiber = new Nutrition()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Nutrition_Item = "Dietary Fiber",
                    DV = 28,
                    Measurement = "g",
                    OrderOnNutrientPanel = 8,
                    ShowOnNutrientPanel = true
                };
                _recitopiaDbContext.Nutrition.Add(dietaryFiber);

                //Sugars
                var sugars = new Nutrition()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Nutrition_Item = "Sugars",
                    DV = 0,
                    Measurement = "g",
                    OrderOnNutrientPanel = 9,
                    ShowOnNutrientPanel = true
                };
                _recitopiaDbContext.Nutrition.Add(sugars);

                //Protein
                var protein = new Nutrition()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Nutrition_Item = "Protein",
                    DV = 50,
                    Measurement = "g",
                    OrderOnNutrientPanel = 10,
                    ShowOnNutrientPanel = true
                };
                _recitopiaDbContext.Nutrition.Add(protein);

                //Vitamin A
                var vitaminA = new Nutrition()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Nutrition_Item = "Vitamin A",
                    DV = 3000,
                    Measurement = "IU",
                    OrderOnNutrientPanel = 11,
                    ShowOnNutrientPanel = true
                };
                _recitopiaDbContext.Nutrition.Add(vitaminA);

                //Vitamin C
                var vitaminC = new Nutrition()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Nutrition_Item = "Vitamin C",
                    DV = 60,
                    Measurement = "mg",
                    OrderOnNutrientPanel = 12,
                    ShowOnNutrientPanel = true
                };
                _recitopiaDbContext.Nutrition.Add(vitaminC);

                //Calcium
                var calcium = new Nutrition()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Nutrition_Item = "Calcium",
                    DV = 1000,
                    Measurement = "mg",
                    OrderOnNutrientPanel = 13,
                    ShowOnNutrientPanel = true
                };
                _recitopiaDbContext.Nutrition.Add(calcium);

                //Iron
                var iron = new Nutrition()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Nutrition_Item = "Iron",
                    DV = 18,
                    Measurement = "mg",
                    OrderOnNutrientPanel = 14,
                    ShowOnNutrientPanel = true
                };
                _recitopiaDbContext.Nutrition.Add(iron);

                //Iron
                var potassium = new Nutrition()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Nutrition_Item = "Potassium",
                    DV = 3500,
                    Measurement = "mg",
                    OrderOnNutrientPanel = 15,
                    ShowOnNutrientPanel = true
                };
                _recitopiaDbContext.Nutrition.Add(potassium);

                //Vitamin D
                var vitaminD = new Nutrition()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Nutrition_Item = "Vitamin D",
                    DV = 15,
                    Measurement = "mcg",
                    OrderOnNutrientPanel = 116,
                    ShowOnNutrientPanel = true
                };
                _recitopiaDbContext.Nutrition.Add(vitaminC);

                //----------------------------------------------------------------------------------------------------
                //--------------END ADD DEFAULT NUTRIENTS-------------------------------------------------------------
                //----------------------------------------------------------------------------------------------------

                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(customers);
        }

        public IActionResult CreateCustomerGroup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomerGroup([FromForm] Customers customers)
        {
            if (ModelState.IsValid)
            {
                customers.Customer_Guid = Guid.NewGuid().ToString();
                _recitopiaDbContext.Add(customers);
                await _recitopiaDbContext.SaveChangesAsync();

                //PRIME VENDOR, SERVING SIZE AND CATEGORY WITH EXAMPLE VALUES SO RECIPES/INGREDIENTS CAN BE CREATED WITH THESE DEPENDENCIES RIGHT AWAY
                //VENDOR
                var vendorPrime = new Vendor()
                {
                    Vendor_Name = "Example Vendor",
                    Notes = "Default Value added as example",
                    Phone = "000-000-0000",
                    Email = "ExampleVendorEmail@gmail.com",
                    Address1 = "1234 Vendor Lane",
                    City = "Recipeville",
                    State = "MN",
                    Zip = 12345,
                    Web_URL = "http://www.examplevendorURL.com",
                    Customer_Guid = customers.Customer_Guid
                };

                _recitopiaDbContext.Vendor.Add(vendorPrime);

                await _recitopiaDbContext.SaveChangesAsync();

                //SERVING SIZE
                var ssPrime = new Serving_Sizes()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Serving_Size = 1,
                    Notes = "Default Value added as example"
                };

                _recitopiaDbContext.Serving_Sizes.Add(ssPrime);

                await _recitopiaDbContext.SaveChangesAsync();

                //CATEGORY
                var categoryPrime = new Meal_Category()
                {
                    Customer_Guid = customers.Customer_Guid,
                    Category_Name = "Snack",
                    Notes = "Default Value added as example"
                };

                _recitopiaDbContext.Meal_Category.Add(categoryPrime);

                await _recitopiaDbContext.SaveChangesAsync();

                


                //ADDED THE CUSTOMER GROUP NOW ADD THE CURRENT USER TO THAT CUSTOMER GROUP AND TAKE TO HOME PAGE
                var currentUser = await _recitopiaDbContext.AppUsers.Where(m => m.UserName == User.Identity.Name).FirstAsync();
                var customerUser = new Customer_Users()
                { 
                    Customer_Name = customers.Customer_Name,
                    User_Name = currentUser.FullName,
                    User_Id = currentUser.Id,
                    Customer_Guid = customers.Customer_Guid,
                    Customer_Id = customers.Customer_Id
                };

                _recitopiaDbContext.Add(customerUser);
                await _recitopiaDbContext.SaveChangesAsync();

                //SET CUSTOMER SESSION
                HttpContext.Session.SetString("CurrentUserCustomerGuid", customers.Customer_Guid);

                //PRIME CURRENT USERS LAST CUSTOMER LOGIN TO NEW CUSTOMER GROUP
                currentUser.Customer_Guid = customers.Customer_Guid;
                currentUser.Customer_Name = customers.Customer_Name;

                _recitopiaDbContext.AppUsers.Update(currentUser);
                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            return View(customers);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _recitopiaDbContext.Customers.FindAsync(id);

            if (customers == null)
            {
                return NotFound();
            }

            return View(customers);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Customers customers)
        {
            if (id != customers.Customer_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _recitopiaDbContext.Update(customers);
                    await _recitopiaDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CustomersExists(customers.Customer_Id))
                    {
                        return NotFound();
                    }

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customers);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _recitopiaDbContext.Customers
                .FirstOrDefaultAsync(m => m.Customer_Id == id);

            if (customers == null)
            {
                return NotFound();
            }

            return View(customers);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customers = await _recitopiaDbContext.Customers.FindAsync(id);


            //NEED TO WRITE MULTIPLE CASCADING DELETES TO MAKE SURE THERE ARE NO ORPHANS
            var customerUsersRemove = await _recitopiaDbContext.Customer_Users.Where(m => m.Customer_Guid == customers.Customer_Guid).ToListAsync();

            _recitopiaDbContext.Customers.Remove(customers);
            await _recitopiaDbContext.SaveChangesAsync();

            _recitopiaDbContext.Customer_Users.RemoveRange(customerUsersRemove);
            await _recitopiaDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CustomersExists(int id)
        {
            return await _recitopiaDbContext.Customers.AnyAsync(e => e.Customer_Id == id);
        }
        
        public async Task<IActionResult> CopyCustomer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _recitopiaDbContext.Customers
                .FirstOrDefaultAsync(m => m.Customer_Id == id);

            if (customers == null)
            {
                return NotFound();
            }

            return View(customers);
        }
        [HttpPost, ActionName("CopyCustomer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CopyCustomerConfirmed(int id)
        {
            //

            var customers = await _recitopiaDbContext.Customers.FindAsync(id);
            try
            {    
                Customers customerNew = new Customers()
                {
                    Customer_Name = "Copy of - " + customers.Customer_Name,
                    Phone = customers.Phone,
                    Email = customers.Email,
                    Address1 = customers.Address1,
                    Address2 = customers.Address2,
                    City = customers.City,
                    State = customers.State,
                    Zip = customers.Zip,
                    Web_URL = customers.Web_URL,
                    Notes = customers.Notes
                };
                await _recitopiaDbContext.Customers.AddAsync(customerNew);
                await _recitopiaDbContext.SaveChangesAsync();

            }
            catch
            {

            }
                return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> RequestAccess(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _recitopiaDbContext.Customers
                .FirstOrDefaultAsync(m => m.Customer_Id == id);

            if (customers == null)
            {
                return NotFound();
            }

            return View(customers);
        }
        [HttpPost, ActionName("RequestAccess")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestAccess(int id)
        {
            
            var customer = await _recitopiaDbContext.Customers.FindAsync(id);

            var currentUser = await _recitopiaDbContext.AppUsers.Where(m => m.UserName == User.Identity.Name).FirstAsync();

            try
            {
                //Already a member of the Customer Group?

                var seeIfAlreadyExists = await _recitopiaDbContext.Customer_Users.Where(m => m.User_Id == currentUser.Id && m.Customer_Guid == customer.Customer_Guid).FirstOrDefaultAsync();

                if (seeIfAlreadyExists == null)
                {
                    var callbackUrl = Url.Action("ConfirmCustomerAddRequest", "Customers", new { user_Id = currentUser.Id, customer_Guid = customer.Customer_Guid }, Request.Scheme);

                    await _emailSender.SendEmailAsync(customer.Email, "Confirm Customer Group Access Request",
                        $"{currentUser.FullName} has requested access to {customer.Customer_Name}.  By <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>, you are approving the request and allowing {currentUser.FullName} access to all your Recipes.");

                    ViewBag.SuccessMessage = "Your request has succesfully been submitted to the Customer Group Owner.";
                }
                else
                {
                    ViewBag.ErrorMessage = "You are already a member of '" + customer.Customer_Name + "'!";
                }
                
            }
            catch
            {
                ViewBag.ErrorMessage = "There was an error submitting your request.";
            }

            return View(customer);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmCustomerAddRequest(string user_Id, string customer_Guid)
        {
            if (user_Id == null || customer_Guid == null)
            {
                ViewBag.ErrorMessage = "Error adding user to Customer Group.";
                return View();
            }
            
            try
            {
                var userInfo = await _recitopiaDbContext.AppUsers.FindAsync(user_Id);
                var customerInfo = await _recitopiaDbContext.Customers.Where(m => m.Customer_Guid == customer_Guid).FirstOrDefaultAsync();

                var customerUser = new Customer_Users()
                {
                    Customer_Name = customerInfo.Customer_Name,
                    User_Name = userInfo.FullName,
                    User_Id = userInfo.Id,
                    Customer_Guid = customerInfo.Customer_Guid,
                    Customer_Id = customerInfo.Customer_Id
                };

                _recitopiaDbContext.Add(customerUser);
                await _recitopiaDbContext.SaveChangesAsync();

                ViewBag.SuccessMessage = "Successfully added user to Customer Group.";
            }
            catch
            {
                ViewBag.ErrorMessage = "Error adding user to Customer Group.";
            }
                


            return View();
        }
    }
}
