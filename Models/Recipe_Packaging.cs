using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recitopia.Models
{
    public partial class Recipe_Packaging
    {
        public int Id { get; set; }
        public int Recipe_Id { get; set; }
        public int Package_Id { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal Weight_g { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }        
        public string Customer_Guid { get; set; }
        public virtual Packaging Packaging { get; set; }
        public virtual Recipe Recipe { get; set; }

    }
}
