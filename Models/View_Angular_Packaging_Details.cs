using System;
using System.Collections.Generic;

namespace Recitopia.Models
{
    public partial class View_Angular_Packaging_Details
    {

        public int Recipe_Id { get; set; }
        public int Package_Id { get; set; }       
        public string Package_Name { get; set; }
        public int Vendor_Id { get; set; }
        public string Vendor_Name { get; set; }       
        public string Website { get; set; }
        public decimal Amount { get; set; }
        public DateTime? LastModified { get; set; }      
        public decimal Cost { get; set; }       
        public decimal Weight { get; set; }      
        public string Notes { get; set; }
        public string Customer_Guid { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
}
