using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class Feedback
    {
       

        public int Id { get; set; }
        [Display(Name = "Subject")]
        [DataType(DataType.Text)]
        [Required]
        public string Subject { get; set; }

        [Display(Name = "Comment")]
        [DataType(DataType.MultilineText)]
        [Required]
        public string Comment { get; set; }
        [Display(Name = "Phone Number(optional)")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [Display(Name = "Email Address(optional)")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        public bool Resolved { get; set; }
        [Display(Name = "Timestamp")]
        [DataType(DataType.DateTime)]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Customer Name")]
        public string Customer_Name { get; set; }
        [Display(Name = "User Name")]
        public string User_Name { get; set; }

        
        [Display(Name = "What are we doing about it?")]
        [DataType(DataType.MultilineText)]
        public string Actions { get; set; }
    }
}
