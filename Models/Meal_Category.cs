using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class Meal_Category
    {
        public Meal_Category()
        {
            Recipe = new HashSet<Recipe>();
        }

        public int Category_Id { get; set; }

        [Display(Name = "Category Name")]
        [Required(ErrorMessage = "Category Name Required")]
        public string Category_Name { get; set; }

        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public int Customer_Id { get; set; }

        public virtual ICollection<Recipe> Recipe { get; set; }
    }
}
