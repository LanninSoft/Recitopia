using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recitopia.Models
{
    public partial class View_All_Plan_Recipes_Details
    {
        public string Recipe_Name { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal CountIt { get; set; }
        
        [Column(TypeName = "decimal(18,3)")]
        public decimal Cost { get; set; }

        public string IngredientName { get; set; }

        public decimal WeightLbs { get; set; }
        public decimal WeightGrams { get; set; }
        
    }
}
