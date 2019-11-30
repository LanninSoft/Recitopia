using System;
using System.Collections.Generic;

namespace Recitopia_LastChance.Models
{
    public partial class Ingredient_Nutrients
    {
        public int Id { get; set; }
        public int Ingred_Id { get; set; }
        public int Nutrition_Item_Id { get; set; }
        public decimal Nut_per_100_grams { get; set; }
        public int Customer_Id { get; set; }

        public virtual Ingredient Ingredients { get; set; }
        public virtual Nutrition Nutrition { get; set; }
    }
}
