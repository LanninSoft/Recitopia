using System;
using System.Collections.Generic;

namespace Recitopia.Models
{
    public partial class Recipe_Ingredients
    {
        public int Id { get; set; }
        public int Recipe_Id { get; set; }
        public int Ingredient_Id { get; set; }
        public decimal Amount_g { get; set; }
        //public int Customer_Id { get; set; }
        public string Customer_Guid { get; set; }
        public virtual Ingredient Ingredient { get; set; }
        public virtual Recipe Recipe { get; set; }

    }
}
