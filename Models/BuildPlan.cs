using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class BuildPlan
    {
        public BuildPlan()
        {
            BuildPlan_Recipes = new HashSet<BuildPlan_Recipes>();
            
        }
        [Key]
        public int BuildPlan_Id { get; set; }

        [Display(Name = "Plan Name")]
        [Required(ErrorMessage = "Plan Name Required")]
        public string Plan_Name { get; set; }

        [Display(Name = "Plan Date")]
        [DataType(DataType.Date)]
        public DateTime PlanDate { get; set; }

        [Display(Name = "Needed By Date")]
        [DataType(DataType.Date)]
        public DateTime NeedByDate { get; set; }

        [Display(Name = "Complete")]
        public bool FullFilled { get; set; }

        
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public DateTime? LastModified { get; set; }
        [Display(Name = "Create Date")]
        public DateTime? CreateDate { get; set; }

        public string Customer_Guid { get; set; }

        public virtual ICollection<BuildPlan_Recipes> BuildPlan_Recipes { get; set; }
    }
}
