using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class RecipeExport
    {
       
        public string Recipe_Name { get; set; }
        public string Category_Name { get; set; }
        public bool Gluten_Free { get; set; } 
        public string SS_Name { get; set; }
        public string SKU { get; set; }
        public string UPC { get; set; }
        public decimal? LaborCost { get; set; }
       
       
    }
}
