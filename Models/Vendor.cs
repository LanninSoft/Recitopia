using System;
using System.Collections.Generic;

namespace Recitopia.Models
{
    public partial class Vendor
    {
        public Vendor()
        {
            Ingredient = new HashSet<Ingredient>();
        }

        public int Vendor_Id { get; set; }
        public string Vendor_Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int? Zip { get; set; }
        public string Web_URL { get; set; }
        public string Notes { get; set; }
        public int Customer_Id { get; set; }

        public virtual ICollection<Ingredient> Ingredient { get; set; }
    }
}
