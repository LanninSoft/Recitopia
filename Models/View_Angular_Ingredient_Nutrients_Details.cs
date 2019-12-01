using System;
using System.Collections.Generic;

namespace Recitopia.Models
{
    public partial class View_Angular_Ingredient_Nutrients_Details
    {
        public long? NID { get; set; }
        public int Id { get; set; }
        public int Customer_Id { get; set; }
        public int Ingred_Id { get; set; }
        public string Ingred_name { get; set; }
        public int Nutrition_Item_Id { get; set; }
        public string Nutrition_Item { get; set; }
        public decimal Nut_per_100_grams { get; set; }
        public int? OrderOnNutrientPanel { get; set; }
        public bool? ShowOnNutrientPanel { get; set; }
    }
}
