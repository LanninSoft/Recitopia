using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recitopia.Models
{
    public partial class View_All_Recipe_Packaging
    {
        
        public int Id { get; set; }
        public string Customer_Guid { get; set; }
        public int Recipe_Id { get; set; }
        public int Package_Id { get; set; }
        public decimal Amount { get; set; }
        public string Package_Name { get; set; }
        public decimal Cost { get; set; }
        public decimal Weight { get; set; }
      
        public string Recipe_Name { get; set; }
        public decimal CountIt { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotalCostPackaging { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotalGrams { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount_lbs { get; set; }
    }
}
