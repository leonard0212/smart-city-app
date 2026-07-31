using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.RawDetection
{
    public class RawDetectionFile
    {

        public  Guid Id { get; set; }
        public FileServiceModel File { get; set; }
    }
}
