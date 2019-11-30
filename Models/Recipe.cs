using System;
using System.Collections.Generic;

namespace Recitopia_LastChance.Models
{
    public partial class Recipe
    {
        public Recipe()
        {
            Recipe_Ingredients = new HashSet<Recipe_Ingredients>();
        }

        public int Recipe_Id { get; set; }
        public string Recipe_Name { get; set; }
        public int Category_Id { get; set; }
        public bool Gluten_Free { get; set; }
        public string SKU { get; set; }
        public string UPC { get; set; }
        public string Notes { get; set; }
        public decimal? LaborCost { get; set; }
        public int SS_Id { get; set; }
        public DateTime? LastModified { get; set; }
        public int Customer_Id { get; set; }

        public virtual Meal_Category Meal_Category { get; set; }
        public virtual Serving_Sizes Serving_Sizes { get; set; }
        public virtual ICollection<Recipe_Ingredients> Recipe_Ingredients { get; set; }
    }
}
