using System;
using System.Collections.Generic;

namespace Recitopia_LastChance.Models
{
    public partial class Serving_Sizes
    {
        public Serving_Sizes()
        {
            Recipe = new HashSet<Recipe>();
        }

        public int SS_Id { get; set; }
        public int Serving_Size { get; set; }
        public string Notes { get; set; }
        public int Customer_Id { get; set; }

        public virtual ICollection<Recipe> Recipe { get; set; }
    }
}
