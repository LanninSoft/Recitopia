using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class RecipeNutritionCompare
    {
     
        public int Recipe_Id { get; set; }

        public string Recipe_Name { get; set; }

        public bool isSelected { get; set; }
    }
}
