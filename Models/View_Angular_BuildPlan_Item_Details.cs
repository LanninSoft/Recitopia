using System;
using System.Collections.Generic;

namespace Recitopia.Models
{
    public partial class View_Angular_BuildPlan_Item_Details
    {

        public int Id { get; set; }
        public int Recipe_Id { get; set; }
        public int BuildPlan_Id { get; set; }
        public decimal Amount { get; set; }
        public string Customer_Guid { get; set; }  
        public string Recipe_Name { get; set; }
    }
}
