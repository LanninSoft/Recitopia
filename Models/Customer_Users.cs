using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Recitopia.Models
{
    public partial class Customer_Users
    {
        [Key]
        public int CU_Id { get; set; }

        [Display(Name = "Customer Id")]
        public int Customer_Id { get; set; }

        
        [Display(Name = "User Id")]
        public string Id { get; set; }

        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public virtual Customers Customers { get; set; }
        public virtual AppUser AppUsers { get; set; }

    }
}
