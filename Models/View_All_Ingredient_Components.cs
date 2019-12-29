using System;
using System.Collections.Generic;

namespace Recitopia.Models
{
    public partial class View_All_Ingredient_Components
    {
        
        public int Id { get; set; }
        
        public string Customer_Guid { get; set; }
        public int Ingred_Id { get; set; }
        public int Comp_Id { get; set; }
        public string Ingred_name { get; set; }
        public string Component_Name { get; set; }
    }
}
