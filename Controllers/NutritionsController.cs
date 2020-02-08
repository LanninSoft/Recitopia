using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class NutritionsController : AuthorizeController
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;

        public NutritionsController(RecitopiaDBContext recitopiaDbContext)
        {
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
        }

        [Authorize]
        public async Task<ActionResult> Index()
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if  (customerGuid == null || customerGuid.Trim() == "")
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            var nutritionList = await _recitopiaDbContext.Nutrition
                .Where(m => m.Customer_Guid == customerGuid)
                .OrderBy(o => o.Nutrition_Item)
                .ToListAsync();

            return View(nutritionList);
        }

        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            var nutritions = await _recitopiaDbContext.Nutrition
                .Where(m => m.Customer_Guid == customerGuid)
                .OrderBy(m => m.Nutrition_Item)
                .ToListAsync();

            return nutritions != null 
                ? Json(nutritions) 
                : Json(new { Status = "Failure" });

        }
        public async Task<IActionResult> ArchiveIt(int? id)
        {

            var nutritionInfo = await _recitopiaDbContext.Nutrition.FindAsync(id);

            var currentUser = await _recitopiaDbContext.AppUsers.Where(m => m.UserName == User.Identity.Name).FirstAsync();

            nutritionInfo.isArchived = true;
            nutritionInfo.ArchiveDate = DateTime.UtcNow;
            nutritionInfo.WhoArchived = currentUser.FullName;

            _recitopiaDbContext.Entry(nutritionInfo).State = EntityState.Modified;

            await _recitopiaDbContext.SaveChangesAsync();

            return RedirectToAction("Details", new { id = nutritionInfo.Nutrition_Item_Id });
        }
        public async Task<IActionResult> UnArchiveIt(int? id)
        {

            var nutritionInfo = await _recitopiaDbContext.Nutrition.FindAsync(id);

            nutritionInfo.isArchived = false;
            nutritionInfo.ArchiveDate = DateTime.MinValue;
            nutritionInfo.WhoArchived = null;

            _recitopiaDbContext.Entry(nutritionInfo).State = EntityState.Modified;

            await _recitopiaDbContext.SaveChangesAsync();

            return RedirectToAction("Details", new { id = nutritionInfo.Nutrition_Item_Id });
        }
        public async Task<ActionResult> Details(int id)
        {
            var nutrition = await _recitopiaDbContext.Nutrition.FindAsync(id);

            if (nutrition == null)
            {
                return NotFound();
            }

            return View(nutrition);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] Nutrition nutrition)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if  (customerGuid == null || customerGuid.Trim() == "")
            {
                return RedirectToAction("CustomerLogin", "Customers");
            }

            if (ModelState.IsValid)
            {
                nutrition.Customer_Guid = customerGuid;
                _recitopiaDbContext.Nutrition.Add(nutrition);
                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(nutrition);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var nutrition = await _recitopiaDbContext.Nutrition.FindAsync(id);

            if (nutrition == null)
            {
                return NotFound();
            }

            return View(nutrition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([FromForm] Nutrition nutrition)
        {
            
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (ModelState.IsValid)
            {
                nutrition.Customer_Guid = customerGuid;
                _recitopiaDbContext.Entry(nutrition).State = EntityState.Modified;
                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(nutrition);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var nutrition = await _recitopiaDbContext.Nutrition.FindAsync(id);

            if (nutrition == null)
            {
                return NotFound();
            }

            return View(nutrition);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var nutrition = await _recitopiaDbContext.Nutrition.FindAsync(id);
           
            try
            {
                _recitopiaDbContext.Nutrition.Remove(nutrition);
                await _recitopiaDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            
            catch (InvalidOperationException) // This will catch SqlConnection Exception
            {
                ViewBag.ErrorMessage = "Uh Oh, There is a problem 2";
                return View(nutrition);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "This Nutrition Item is associated with Ingredient(s) and cannot be deleted.";
                return View(nutrition);
            }
        }
        [HttpGet]
        public async Task<IActionResult> DownloadNutritions()
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            var customerInfo = await _recitopiaDbContext.Customers.Where(m => m.Customer_Guid == customerGuid).SingleAsync();

            var nutritionList = await _recitopiaDbContext.Nutrition
                .Where(p => p.Customer_Guid == customerGuid)
                .OrderByDescending(p => p.Nutrition_Item)
                .Select(nutritions =>
                    new NutritionExport()
                    {
                        Nutrition_Item = nutritions.Nutrition_Item,
                        DV = nutritions.DV,
                        Measurement = nutritions.Measurement,
                        OrderOnNutrientPanel = nutritions.OrderOnNutrientPanel,
                        ShowOnNutrientPanel = nutritions.ShowOnNutrientPanel,                        

                    }

                ).ToListAsync();

            List<NutritionExport> CSVModels = nutritionList;

            var stream = new MemoryStream();
            var writeFile = new StreamWriter(stream);
            var csv = new CsvWriter(writeFile, System.Globalization.CultureInfo.CurrentCulture);
            //csv.Configuration.RegisterClassMap<GroupReportCSVMap>();

            csv.WriteRecords(CSVModels);
            writeFile.Flush();

            stream.Position = 0; //reset stream
            return File(stream, "application/octet-stream", customerInfo.Customer_Name + "_Nutritions.csv");
        }
        public IActionResult uploadNutritionFile(int? id)
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            var uploadFiles = new UploadFiles()
            {
                customerId = customerGuid,

            };

            return View(uploadFiles);
        }
        [HttpPost]
        public async Task<IActionResult> uploadNutritionFile([FromForm] UploadFiles uploadFiles, IFormFile file)
        {
            var customerGuid = HttpContext.Session.GetString("CurrentUserCustomerGuid");

            if (file != null && file.Length > 0 && file.FileName.Contains(".csv"))
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    string s = Convert.ToBase64String(fileBytes);
                    // act on the Base64 data

                    //Convert string into a MemoryStream
                    Stream stream = new MemoryStream(fileBytes);

                    //Parse the stream
                    using (TextFieldParser parser = new TextFieldParser(stream))
                    {
                        parser.TextFieldType = FieldType.Delimited;
                        //parser.TextFieldType = FieldType.Delimited;
                        parser.SetDelimiters(",");
                        int rowCount = 1, colCount = 1;
                        string Field1 = "", Field2 = "", Field3 = "", Field4 = "", Field5 = "";
                        while (!parser.EndOfData)
                        {
                            //Processing row
                            string[] row = parser.ReadFields();
                            if (rowCount > 1) //Skip header row
                            {
                                foreach (string field in row)
                                {
                                    if (colCount == 1)
                                    {
                                        Field1 = field;
                                    }
                                    else if (colCount == 2)
                                    {
                                        Field2 = field;
                                    }
                                    else if (colCount == 3)
                                    {
                                        Field3 = field;
                                    }
                                    else if (colCount == 4)
                                    {
                                        Field4 = field;
                                    }
                                    else if (colCount == 5)
                                    {
                                        Field5 = field;
                                    }
                                    
                                    colCount++;
                                }
                                colCount = 1;
                                //SEE IF VALID DATA AND TRY TO INSERT
                                try
                                {
                                   
                                    var nutrition = new Nutrition()
                                    {
                                        Nutrition_Item = Field1,
                                        DV = ((Field2 != null && Field2.Trim() != "") ? int.Parse(Field2) : 0),
                                        Measurement = Field3,
                                        OrderOnNutrientPanel = ((Field4 != null && Field4.Trim() != "") ? int.Parse(Field4) : 0),
                                        ShowOnNutrientPanel = Convert.ToBoolean(Field5),
                                        Customer_Guid = customerGuid,


                                    };

                                    await _recitopiaDbContext.Nutrition.AddAsync(nutrition);
                                    await _recitopiaDbContext.SaveChangesAsync();
                                }
                                catch (Exception)
                                {
                                    ViewBag.ErrorMessage = "There is an issue with the data in row - " + rowCount + ".  Rows prior to this error were imported.  Fix the data issue, adjust your import file rows to import and try again.";
                                    return View(uploadFiles);
                                }


                            }

                            rowCount++;
                        }
                    }



                }
            }
            else
            {
                ViewBag.ErrorMessage = "The file is missing, is larger than 4mb, or is not of type CSV.  Please re-select the CSV file you wish to upload and try again.";
                return View(uploadFiles);
            }



            return RedirectToAction("Index");
        }
    }
}
