using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Users
{
    public class PasswordHistory : EntityComplex<Guid>
    {
        protected PasswordHistory() { }

        public PasswordHistory(User user)
        {
            User = user;
            PasswordHash = user.PasswordHash;
            SecurityStamp = user.SecurityStamp;
        }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public virtual string PasswordHash { get; set; }

        public virtual string SecurityStamp { get; set; }
    }
}
