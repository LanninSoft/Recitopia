using System;
using System.Collections.Generic;

namespace Recitopia.Models
{
    public partial class View_Angular_Recipe_Ingredients_Details
    {
        
        public int Id { get; set; }
        public int Customer_Id { get; set; }
        public int Ingredient_Id { get; set; }
        public string Ingred_name { get; set; }
        public int Recipe_Id { get; set; }
        public string Recipe_Name { get; set; }
        public decimal Amount_g { get; set; }
    }
}
