using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class Nutrition
    {
        public Nutrition()
        {
            Ingredient_Nutrients = new HashSet<Ingredient_Nutrients>();
        }

        public int Nutrition_Item_Id { get; set; }

        [Required(ErrorMessage = "Nutrition Name Required")]
        [Display(Name = "Nutrition Name")]
        public string Nutrition_Item { get; set; }
       
        [Display(Name = "Daily Value")]        
        public int? DV { get; set; }

        public string Measurement { get; set; }

        [Display(Name = "Order On Nutrition Panel")]
        public int? OrderOnNutrientPanel { get; set; }

        [Display(Name = "Show On Nutrition Panel")]
        public bool ShowOnNutrientPanel { get; set; }
        //public int Customer_Id { get; set; }
        public string Customer_Guid { get; set; }
        public bool isArchived { get; set; }
        public DateTime ArchiveDate { get; set; }
        public string WhoArchived { get; set; }

        public virtual ICollection<Ingredient_Nutrients> Ingredient_Nutrients { get; set; }
    }
}
