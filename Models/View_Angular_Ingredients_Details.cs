using System;
using System.Collections.Generic;

namespace Recitopia.Models
{
    public partial class View_Angular_Ingredients_Details
    {
        
        public int Ingredient_Id { get; set; }
        public int Customer_Id { get; set; }
        public string Ingred_name { get; set; }
        public decimal? Cost_per_lb { get; set; }
        public decimal? Cost { get; set; }
        public bool Package { get; set; }
        public string Vendor_Name { get; set; }
    }
}
