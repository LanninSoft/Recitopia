using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recitopia.Models
{
    public partial class BuildPlan_Recipes
    {
        public int Id { get; set; }
        public int Recipe_Id { get; set; }
        public int BuildPlan_Id { get; set; }

        [Display(Name = "Amount")]
        [Column(TypeName = "decimal(18,0)")]
        public decimal Amount { get; set; }        
        public string Customer_Guid { get; set; }

        public string Recipe_Name { get; set; }


        public virtual BuildPlan BuildPlan { get; set; }
        public virtual Recipe Recipe { get; set; }


    }
}
