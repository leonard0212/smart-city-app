using SmartCity.Domain.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SmartCity.Domain.Models
{
    public abstract class EntityComplex<TId> : Entity<TId>
    {
        protected EntityComplex()
        {
            Enabled = true;

        }

        [JsonIgnore]
        public bool Enabled { get; set; }

        [JsonIgnore]
        public Guid? CreatedById { get; protected set; }

        [JsonIgnore]
        public User CreatedBy { get; protected set; }

        [JsonIgnore]
        public Guid? UpdatedById { get; protected set; }

        [JsonIgnore]
        public User UpdatedBy { get; protected set; }

        [JsonIgnore]
        [ConcurrencyCheck]
        public DateTime? UpdatedAt { get; protected set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public Guid? DeletedById { get; protected set; }

        [JsonIgnore]
        public User DeletedBy { get; protected set; }

        [JsonIgnore]
        public DateTime? DeletedAt { get; protected set; }

        public void PreInsert(User currentUser, DateTime datetimeNow)
        {
            CreatedBy = currentUser;
            CreatedAt = datetimeNow;
        }

        public void PreUpdate(User currentUser, DateTime datetimeNow)
        {
            UpdatedBy = currentUser;
            UpdatedAt = datetimeNow;
        }

        public void ToggleEnabled(bool? enable, User updatedBy, DateTime datetimeNow)
        {
            Enabled = enable.HasValue ? enable.Value : !Enabled;
            PreUpdate(updatedBy, datetimeNow);
        }

        public void SoftDelete(User currentUser, DateTime datetimeNow)
        {
            DeletedBy = currentUser;
            DeletedAt = datetimeNow;
            IsDeleted = true;
        }

        public void UnSoftDelete()
        {
            DeletedBy = null;
            DeletedAt = null;
            IsDeleted = false;
        }
    }
}
