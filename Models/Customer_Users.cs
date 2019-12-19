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

        [Display(Name = "Customer Name")]
        public int Customer_Id { get; set; }
        public string Customer_Name { get; set; }
        
        [Display(Name = "User Name")]
        public string Id { get; set; }
        public string User_Name { get; set; }

        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public virtual AppUser AppUser { get; set; }
        public virtual Customers Customers { get; set; }
    }
}
