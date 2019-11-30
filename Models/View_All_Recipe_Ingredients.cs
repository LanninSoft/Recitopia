using System;
using System.Collections.Generic;

namespace Recitopia_LastChance.Models
{
    public partial class View_All_Recipe_Ingredients
    {
        public long? NID { get; set; }
        public int Id { get; set; }
        public int Customer_Id { get; set; }
        public int Recipe_Id { get; set; }
        public int Ingredient_Id { get; set; }
        public decimal Amount_g { get; set; }
        public string Ingred_name { get; set; }
        public string Ingred_Comp_name { get; set; }
        public decimal? Cost_per_lb { get; set; }
        public decimal? Cost { get; set; }
        public bool? Package { get; set; }
        public string Recipe_Name { get; set; }
    }
}
