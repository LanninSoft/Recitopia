using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class NutritionExport
    {
        
        public string Nutrition_Item { get; set; }      
        public int? DV { get; set; }
        public string Measurement { get; set; }     
        public int? OrderOnNutrientPanel { get; set; }       
        public bool ShowOnNutrientPanel { get; set; }
       

    }
}
