namespace SmartCity.Domain.ServiceModels.File
{
    public class CreateFileAzureUploadRequest : CreateFileAzureDownloadRequest
    {
        public string Base64Content { get; set; }
    }
}
