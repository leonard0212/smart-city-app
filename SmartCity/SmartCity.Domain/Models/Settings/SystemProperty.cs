namespace SmartCity.Domain.Models.Settings
{
    public class SystemProperty : Entity<long>
    {

        public SystemProperty()
        {
            CreatedAt = DateTime.Now;
        }


        public string Name { get; set; }
        public string Value { get; set; }


        public DateTime CreatedAt { get; set; }
    }
}
