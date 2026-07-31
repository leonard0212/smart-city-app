using System;

namespace SmartCity.Web.Models.UserManagement
{
    public class UserModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool TwoFactorEnabled { get; set; }


        public DateTimeOffset? LockoutEnd { get; set; }

        public bool LockoutEnabled { get; set; }

        public string Account { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public bool Enabled { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<string> Roles { get; set; }
        public List<string> Claims { get; set; }


        public string CreatedBy { get; set; }
    }
}
