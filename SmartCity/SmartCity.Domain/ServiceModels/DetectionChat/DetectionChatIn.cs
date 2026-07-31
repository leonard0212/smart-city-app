using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.DetectionChat
{
    public class DetectionChatIn
    {
        public Guid DetectionId { get; set; }

        public string? Text { get; set; }
    }
}
