using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Recitopia.Models
{
    public partial class Customers
    {

        public Customers()
        {
            Customer_Users = new HashSet<Customer_Users>();
        }
        [Key]
        public int Customer_Id { get; set; }

        [Display(Name = "Customer Name")]
        public string Customer_Name { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string Phone { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Address 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }
        
        [Display(Name = "Postal Code")]
        public int? Zip { get; set; }

        [Display(Name = "Web URL")]
        public string Web_URL { get; set; }

        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }       

        public virtual ICollection<Customer_Users> Customer_Users { get; set; }

    }
}
