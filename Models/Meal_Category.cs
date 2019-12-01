using System;
using System.Collections.Generic;

namespace Recitopia.Models
{
    public partial class Meal_Category
    {
        public Meal_Category()
        {
            Recipe = new HashSet<Recipe>();
        }

        public int Category_Id { get; set; }
        public string Category_Name { get; set; }
        public string Notes { get; set; }
        public int Customer_Id { get; set; }

        public virtual ICollection<Recipe> Recipe { get; set; }
    }
}
