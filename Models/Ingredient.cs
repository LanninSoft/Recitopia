using System;
using System.Collections.Generic;

namespace Recitopia.Models
{
    public partial class Ingredient
    {
        public Ingredient()
        {
            Ingredient_Components = new HashSet<Ingredient_Components>();
            Ingredient_Nutrients = new HashSet<Ingredient_Nutrients>();
            Recipe_Ingredients = new HashSet<Recipe_Ingredients>();
        }

        public int Ingredient_Id { get; set; }
        public string Ingred_name { get; set; }
        public string Ingred_Comp_name { get; set; }
        public decimal? Cost_per_oz { get; set; }
        public decimal? Cost_per_gram { get; set; }
        public decimal? Cost_per_cup { get; set; }
        public decimal? Cost_per_lb { get; set; }
        public decimal? Cost_per_tsp { get; set; }
        public decimal? Cost_per_tbsp { get; set; }
        public decimal? Per_item { get; set; }
        public decimal? Weight_Equiv_g { get; set; }
        public string Weight_Equiv_measure { get; set; }
        public int Vendor_Id { get; set; }
        public string Vendor_name { get; set; }
        public string Website { get; set; }
        public string Notes { get; set; }
        public decimal? Cost_per_lb2 { get; set; }
        public decimal? Cost_per_ounce2 { get; set; }
        public decimal? Cost_per_gram2 { get; set; }
        public string Packaging { get; set; }
        public string Brand { get; set; }
        public DateTime? LastModified { get; set; }
        public bool Package { get; set; }
        public decimal? Cost { get; set; }
        public int Customer_Id { get; set; }

        public virtual Vendor Vendor { get; set; }
        public virtual ICollection<Ingredient_Components> Ingredient_Components { get; set; }
        public virtual ICollection<Ingredient_Nutrients> Ingredient_Nutrients { get; set; }
        public virtual ICollection<Recipe_Ingredients> Recipe_Ingredients { get; set; }
    }
}
