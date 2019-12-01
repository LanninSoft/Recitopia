using System;
using System.Collections.Generic;

namespace Recitopia.Models
{
    public partial class Nutrition
    {
        public Nutrition()
        {
            Ingredient_Nutrients = new HashSet<Ingredient_Nutrients>();
        }

        public int Nutrition_Item_Id { get; set; }
        public string Nutrition_Item { get; set; }
        public int? DV { get; set; }
        public string Measurement { get; set; }
        public int? OrderOnNutrientPanel { get; set; }
        public bool ShowOnNutrientPanel { get; set; }
        public int Customer_Id { get; set; }

        public virtual ICollection<Ingredient_Nutrients> Ingredient_Nutrients { get; set; }
    }
}
