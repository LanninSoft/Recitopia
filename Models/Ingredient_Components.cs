﻿using System;
using System.Collections.Generic;

namespace Recitopia.Models
{
    public partial class Ingredient_Components
    {
        public int Id { get; set; }
        public int Ingred_Id { get; set; }
        public int Comp_Id { get; set; }
        //public int Customer_Id { get; set; }
        public string Customer_Guid { get; set; }

        public virtual Components Components { get; set; }
        public virtual Ingredient Ingredients { get; set; }
    }
}
