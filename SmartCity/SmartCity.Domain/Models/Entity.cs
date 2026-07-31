using System.Text.Json.Serialization;

namespace SmartCity.Domain.Models
{
    public abstract class Entity<TId>
    {

        public Entity()
        {
            CreatedAt = DateTime.Now;
        }

        public virtual TId Id { get; protected set; }

        [JsonIgnore]
        public DateTime CreatedAt { get; protected set; }
    }
}
