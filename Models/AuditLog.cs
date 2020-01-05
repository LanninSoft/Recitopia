using System;
using System.ComponentModel.DataAnnotations;

namespace Recitopia.Models
{
    public partial class AuditLog
    {
        
        [Key]
        public int Id { get; set; }
        [Required]
        public string Event { get; set; }
        [Required]
        public DateTime EventDate { get; set; }
        [Required]
        public string UserId { get; set; }

        public string WhatChanged { get; set; }

        public string CustomerGuid { get; set; }

    



    }
}
