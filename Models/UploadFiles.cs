using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class UploadFiles
    {

        [Key]
        public int id { get; set; }
        [Display(Name = "File Name")]
        
        public string FileName { get; set; }
        
        [MaxLength]
        [DataType(DataType.Upload)]
        [Required]
        public byte[] File { get; set; }
        public string FileContentType { get; set; }
       
        public string customerId { get; set; }


    }
   
}
