using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class View_Recipe_Details
    {
        public View_Recipe_Details()
        {
            Recipe_Ingredients = new HashSet<Recipe_Ingredients>();
            Recipe_Packaging = new HashSet<Recipe_Packaging>();
        }

        public int Recipe_Id { get; set; }

        [Display(Name = "Recipe Name")]
        [Required(ErrorMessage = "Recipe Name Required")]
        public string Recipe_Name { get; set; }      
        public string Category_Name { get; set; }
        [Display(Name = "Gluten Free")]
        public bool Gluten_Free { get; set; }
        public string SKU { get; set; }
        public string UPC { get; set; }
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        [Display(Name = "Labor Cost($)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? LaborCost { get; set; }
        [Display(Name = "Retail Price($)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal RetailPrice { get; set; }
        [Display(Name = "Wholesale Price($)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WholesalePrice { get; set; }       
        public string SS_Name { get; set; }
        public DateTime? LastModified { get; set; }
        public bool isArchived { get; set; }
        public DateTime ArchiveDate { get; set; }      
        public int Scaleit_Amount { get; set; } = 1;
        public virtual Meal_Category Meal_Category { get; set; }
        public virtual Serving_Sizes Serving_Sizes { get; set; }
        public virtual ICollection<Recipe_Ingredients> Recipe_Ingredients { get; set; }
        public virtual ICollection<Recipe_Packaging> Recipe_Packaging { get; set; }

    }
}
