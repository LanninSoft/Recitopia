using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class NutritionPanelItem
    {
            
        public string Nutrition_Item { get; set; }
              
        public int DV { get; set; }

        public decimal Nut_per_100_grams { get; set; }
    }
}
