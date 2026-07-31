using SmartCity.Domain.Models.Enums;

namespace SmartCity.Domain.ServiceModels.File
{
    public class CreateFileIn
    {
        public string Category { get; set; }
        public string FileName { get; set; }
        public byte[] File { get; set; }
        public string ContentType { get; set; }

    }
}
