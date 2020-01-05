using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class Feedback
    {
        public Feedback()
        {
            FeedbackFiles = new HashSet<FeedbackFiles>();
        }
        public virtual ICollection<FeedbackFiles> FeedbackFiles { get; set; }

        public int Id { get; set; }

        [Display(Name = "Subject")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Subject is Required.")]
        public string Subject { get; set; }

        [Display(Name = "Comment")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Comment(s) Required.")]
        public string Comment { get; set; }
        [Display(Name = "Phone Number (optional)")]
        [Phone]
        public string PhoneNumber { get; set; }
        [Display(Name = "Email Address (optional)")]
        [EmailAddress(ErrorMessage = "Improper Email format.")]
        public string EmailAddress { get; set; }

        public bool Resolved { get; set; }
        [Display(Name = "Initial Submission")]
        [DataType(DataType.DateTime)]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Customer Name")]
        public string Customer_Name { get; set; }
        [Display(Name = "User Name")]
        public string User_Name { get; set; }
        public string User_Id { get; set; }


        [Display(Name = "What are we doing about it?")]
        [DataType(DataType.MultilineText)]
        public string Actions { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime ResolvedDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
