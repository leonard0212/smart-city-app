using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SmartCity.Domain.ServiceModels
{
    public class FileServiceModel
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string Extension { get; set; }
        public byte[] Content { get; set; }
        public string Category { get; set; }
        public string Base64Content => Convert.ToBase64String(Content);
    }
}
