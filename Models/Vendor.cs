using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class Vendor
    {
        public Vendor()
        {
            Ingredient = new HashSet<Ingredient>();
        }

        public int Vendor_Id { get; set; }

        [Display(Name = "Vendor Name")]
        [Required(ErrorMessage = "Vendor Name Required.")]
        public string Vendor_Name { get; set; }
       
        [Display(Name = "Phone number")]
        [Phone(ErrorMessage = "Improper Phone format. (1-234-567-8912)")]
        public string Phone { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Improper Email format.")]
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
        [DataType(DataType.PostalCode, ErrorMessage = "Improper Postal Code format.")]
        public int? Zip { get; set; }

        [Display(Name = "Web URL")]
        [Url(ErrorMessage = "Improper URL format. (http://www.example.com)")]
        public string Web_URL { get; set; }

        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        //public int Customer_Id { get; set; }
        public string Customer_Guid { get; set; }
        public virtual ICollection<Ingredient> Ingredient { get; set; }
        public virtual ICollection<Packaging> Packaging { get; set; }
    }
}
