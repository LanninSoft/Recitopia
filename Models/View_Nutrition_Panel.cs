using System;
using System.Collections.Generic;

namespace Recitopia_LastChance.Models
{
    public partial class View_Nutrition_Panel
    {
        public long? NID { get; set; }
        public int Recipe_Id { get; set; }
        public int Customer_Id { get; set; }
        public string Recipe_Name { get; set; }
        public int? Serving_Size { get; set; }
        public int? Ingred_Id { get; set; }
        public string Ingred_name { get; set; }
        public bool? ShowOnNutrientPanel { get; set; }
        public int? OrderOnNutrientPanel { get; set; }
        public int? Nutrition_Item_Id { get; set; }
        public decimal? Nut_per_100_grams { get; set; }
        public decimal Amount_g { get; set; }
        public string Nutrition_Item { get; set; }
        public int? DV { get; set; }
        public string Measurement { get; set; }
    }
}
