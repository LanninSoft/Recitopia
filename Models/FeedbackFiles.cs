using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class FeedbackFiles
    {


        [Key]
        public int ImageId { get; set; }
        [Display(Name = "File Name")]
        [Required]
        public string FileName { get; set; }
        
        [MaxLength]
        [DataType(DataType.Upload)]
        public byte[] Image { get; set; }
        public string FileContentType { get; set; }
        public int FeedbackId { get; set; }
        public string FeedbackSubject { get; set; }


    }
   
}
