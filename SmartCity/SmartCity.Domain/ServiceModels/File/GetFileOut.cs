using SmartCity.Domain.Models.Enums;

namespace SmartCity.Domain.ServiceModels.File
{
    public class GetFileOut
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string Extension { get; set; }
        public byte[] Content { get; set; }
        public string Category { get; set; }
    }
}
