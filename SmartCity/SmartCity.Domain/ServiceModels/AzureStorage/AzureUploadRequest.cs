using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.AzureStorage
{
    public class AzureUploadRequest : AzureDownloadRequest
    {
        public required string Base64Content { get; set; }
    }
}
