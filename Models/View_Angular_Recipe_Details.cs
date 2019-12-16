using System;
using System.Collections.Generic;

namespace Recitopia.Models
{
    public partial class View_Angular_Recipe_Details
    {
        
        public int Recipe_Id { get; set; }
        public int Customer_Id { get; set; }
        public string Recipe_Name { get; set; }
        public int Category_Id { get; set; }
        public string Category_Name { get; set; }
        public bool Gluten_Free { get; set; }
        public string SKU { get; set; }
        public string UPC { get; set; }
        public string Notes { get; set; }
        public decimal? LaborCost { get; set; }
        public int SS_Id { get; set; }
        public int? Serving_Size { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
