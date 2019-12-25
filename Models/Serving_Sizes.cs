using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class Serving_Sizes
    {
        public Serving_Sizes()
        {
            Recipe = new HashSet<Recipe>();
        }

        public int SS_Id { get; set; }
        [Display(Name = "Serving Size")]
        [Required(ErrorMessage = "Serving Size Required")]
        public int Serving_Size { get; set; }
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public int Customer_Id { get; set; }

        public virtual ICollection<Recipe> Recipe { get; set; }
    }
}
