using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class Components
    {
        public Components()
        {
            Ingredient_Components = new HashSet<Ingredient_Components>();
        }
        
        public int Comp_Id { get; set; }
        [Required(ErrorMessage = "Allergen Name Required.")]
        [Display(Name = "Allergen Name")]
        public string Component_Name { get; set; }
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public string Comp_Sort { get; set; }
       
        public string Customer_Guid { get; set; }

        public virtual ICollection<Ingredient_Components> Ingredient_Components { get; set; }
    }
}
