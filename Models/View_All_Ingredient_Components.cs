﻿using System;
using System.Collections.Generic;

namespace Recitopia.Models
{
    public partial class View_All_Ingredient_Components
    {
        public long? NID { get; set; }
        public int Id { get; set; }
        public int Customer_Id { get; set; }
        public int Ingred_Id { get; set; }
        public int Comp_Id { get; set; }
        public string Ingred_name { get; set; }
        public string Component_Name { get; set; }
    }
}