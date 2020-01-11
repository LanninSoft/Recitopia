using System;
using System.Collections.Generic;

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
    }
}
