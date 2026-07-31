using SmartCity.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Interfaces.Services
{
    public interface IDetectionFlowLogService
    {
        Task CreateDetectionFlowLog(Guid detectionId, Guid userId, string logText);
        Task<List<DetectionFlowLog>> GetDetectionFlowLog(Guid detectionId);
    }
}
