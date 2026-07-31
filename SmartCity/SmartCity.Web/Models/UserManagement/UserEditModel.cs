
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace SmartCity.Web.Models.UserManagement
{
    public class UserEditModel
    {
        [HiddenInput]
        public long Id { get; set; }
        public long AppUserId { get; set; }        
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public string AccountName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Enabled { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string> Claims { get; set; }
        public List<string> Roles { get; set; }
    }
}
