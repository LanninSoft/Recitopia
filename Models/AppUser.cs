using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Recitopia.Models
{
    public class AppUser : IdentityUser<string>
    {

    public string Id { get; set; }

    public string UserName { get; set; }
    public string NormalizedUserName { get; set; }
    public string Email { get; set; }
    public string NormalizedEmail { get; set; }
    public string EmailConfirmed { get; set; }
    public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string TwoFactorEnabled { get; set; }
        public string LockoutEnd] DATETIMEOFFSET(7) NULL,
    public string LockoutEnabled] BIT NOT NULL,
    public string AccessFailedCount] INT NOT NULL,
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string WebUrl { get; set; }
        public string Notes { get; set; }
        public int Customer_Id { get; set; }
    }
}
