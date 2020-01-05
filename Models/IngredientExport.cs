using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class IngredientExport
    {
       
        public string Ingred_name { get; set; }
        public string Ingred_Comp_name { get; set; }       
        public decimal? Cost_per_gram { get; set; }       
        public decimal? Cost_per_lb { get; set; }
        public string Vendor_name { get; set; }        
        public string Website { get; set; }       
        public string Notes { get; set; }       
        public string Packaging { get; set; }
        public string Brand { get; set; }
       
        public bool Package { get; set; }
        public decimal? Cost { get; set; }
       
    }
}
