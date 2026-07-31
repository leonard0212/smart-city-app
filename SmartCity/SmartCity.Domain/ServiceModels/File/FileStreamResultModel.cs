using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.File
{
    public class FileStreamResultModel
    {
        public Stream Stream { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string Category { get; set; }
        public string Extension { get; set; }
    }
}