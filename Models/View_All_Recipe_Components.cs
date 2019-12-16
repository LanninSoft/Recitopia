using System;
using System.Collections.Generic;

namespace Recitopia.Models
{
    public partial class View_All_Recipe_Components
    {
        
        public int Recipe_Id { get; set; }
        public int Customer_Id { get; set; }
        public string Recipe_Name { get; set; }
        public int? Ingredient_Id { get; set; }
        public decimal? Amount_g { get; set; }
        public int? Comp_Id { get; set; }
        public string Component_Name { get; set; }
    }
}
