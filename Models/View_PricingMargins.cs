using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class View_PricingMargins
    {
        
        
        public string Item { get; set; }
        public string Recipe_Name { get; set; }    

        public int Recipe_Id { get; set; }

        [Display(Name = "Recipe Cost($)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal RecipeCost { get; set; }

        [Display(Name = "Labor($)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Labor { get; set; }

        [Display(Name = "Total Cost($)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal TotalCost { get; set; }

        [Display(Name = "Retail Price($)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal RetailPrice { get; set; }

        [Display(Name = "Retail GP($)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal RetailGP { get; set; }

        [Display(Name = "Retail GP margin(%)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal RetailGPMargin { get; set; }

        [Display(Name = "Wholesale Price($)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WholesalePrice { get; set; }

        [Display(Name = "Wholesale GP($)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WholesaleGP { get; set; }

        [Display(Name = "Wholesale GP margin(%)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WholesaleGPMargin { get; set; }

    }
}
