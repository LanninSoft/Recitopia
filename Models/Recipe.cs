using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class Recipe
    {
        public Recipe()
        {
            Recipe_Ingredients = new HashSet<Recipe_Ingredients>();
            Recipe_Packaging = new HashSet<Recipe_Packaging>();
        }

        public int Recipe_Id { get; set; }

        [Display(Name = "Recipe Name")]
        [Required(ErrorMessage = "Recipe Name Required")]
        public string Recipe_Name { get; set; }

        [Display(Name = "Meal Category")]
        public int Category_Id { get; set; }
        public string Category_Name { get; set; }

        [Display(Name = "Gluten Free")]
        public bool Gluten_Free { get; set; }
        public string SKU { get; set; }
        public string UPC { get; set; }

        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Display(Name = "Labor Cost")]
        [DataType(DataType.Currency)]
        public decimal? LaborCost { get; set; }
        [Display(Name = "Serving Size")]
        public int SS_Id { get; set; }
        public string SS_Name { get; set; }
        public DateTime? LastModified { get; set; }
       // public int Customer_Id { get; set; }
        public string Customer_Guid { get; set; }
        public virtual Meal_Category Meal_Category { get; set; }
        public virtual Serving_Sizes Serving_Sizes { get; set; }
        public virtual ICollection<Recipe_Ingredients> Recipe_Ingredients { get; set; }
        public virtual ICollection<Recipe_Packaging> Recipe_Packaging { get; set; }
    }
}
