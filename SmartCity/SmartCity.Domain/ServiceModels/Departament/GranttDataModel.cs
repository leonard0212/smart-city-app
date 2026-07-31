namespace SmartCity.Domain.ServiceModels.Departament
{
    public class GranttDataModel
    {
        public string TaskId { get; set; }
        public string TaskName { get; set; }
        public string Resource { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int? Duration { get; set; }
        public double PercentComplete { get; set; }
        public string? Dependencies { get; set; }
    }
}
