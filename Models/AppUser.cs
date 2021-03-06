﻿using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{

    public class AppUser : IdentityUser<string>
    {
        [Display(Name = "First Name")]        
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]       
        public string LastName { get; set; }
        [Display(Name = "Address 1")]
        public string Address1 { get; set; }
        [Display(Name = "Address 2")]
        public string Address2 { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Postal Code")]
        [DataType(DataType.PostalCode)]
        public string ZipCode { get; set; }
        [Display(Name = "Web URL")]
        [Url]
        public string WebUrl { get; set; }

        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        [Display(Name = "Role")]
        public string Site_Role_Id { get; set; }
       
        public string Customer_Guid { get; set; }
        public string Customer_Name { get; set; }

        public IEnumerable<Customers> Customers { get; set; }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}
