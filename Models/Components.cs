using System;
using System.Collections.Generic;

namespace Recitopia.Models
{
    public partial class Components
    {
        public Components()
        {
            Ingredient_Components = new HashSet<Ingredient_Components>();
        }

        public int Comp_Id { get; set; }
        public string Component_Name { get; set; }
        public string Notes { get; set; }
        public string Comp_Sort { get; set; }

        public virtual ICollection<Ingredient_Components> Ingredient_Components { get; set; }
    }
}
