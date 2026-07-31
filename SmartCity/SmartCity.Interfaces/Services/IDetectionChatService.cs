using SmartCity.Domain.ServiceModels.DetectionChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Interfaces.Services
{
    public interface IDetectionChatService
    {
        public Task AddDetectionChatAsync(DetectionChatIn data);
        public Task<List<DetectionChatOut>> GetChatByDetectionIdAsync(Guid detectionId);
    }
}
