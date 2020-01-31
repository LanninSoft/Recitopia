using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recitopia.Models
{
    public partial class View_Angular_Ingredients_Details
    {
        
        public int Ingredient_Id { get; set; }
        public string Customer_Guid { get; set; }
        public string Ingred_name { get; set; }
        public decimal? Cost_per_lb { get; set; }
        public decimal? Cost { get; set; }
        public bool Package { get; set; }
        public string Vendor_Name { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Amount_g { get; set; }
        public int recipe_Id { get; set; }
    }
}
