using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recitopia.Models
{
    public partial class Packaging
    {
        public Packaging()
        {            
            Recipe_Packaging = new HashSet<Recipe_Packaging>();
        }

        [Key]
        public int Package_Id { get; set; }
        [Display(Name = "Package Name")]
        [Required(ErrorMessage = "Package Name Required")]
        public string Package_Name { get; set; }  
        
        public int Vendor_Id { get; set; }
        [Display(Name = "Vendor Name")]
        public string Vendor_Name { get; set; }
        [Display(Name = "URL")]
        [Url]
        public string Website { get; set; }
        
        public DateTime? LastModified { get; set; }

        [Display(Name = "Cost($)")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,3)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal Cost { get; set; }

        [Display(Name = "Weight(g)")]
        [Column(TypeName = "decimal(18,3)")]
        public decimal Weight { get; set; }

        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public string Customer_Guid { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual ICollection<Recipe_Packaging> Recipe_Packaging { get; set; }
    }
}
