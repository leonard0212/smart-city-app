using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.AzureStorage
{
    public class AzureDownloadRequest
    {
        public required string ApplicationName { get; set; }
        public required string FileName { get; set; }
        public required string FolderName { get; set; }
    }
}
