using Microsoft.EntityFrameworkCore;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces.Services;

namespace SmartCity.Core.Services
{
    public class DetectionFlowLogService : ServiceBase, IDetectionFlowLogService
    {
        private readonly IGenericRepositorySimpleUniqueIdentifier<DetectionFlowLog> _detectionFlowLogRepository;

        public DetectionFlowLogService(IApplicationContext<User> applicationContext, IGenericRepositorySimpleUniqueIdentifier<DetectionFlowLog> detectionFlowLogRepository) : base(applicationContext)
        {
            _detectionFlowLogRepository = detectionFlowLogRepository;
        }

        public async Task CreateDetectionFlowLog(Guid detectionId, Guid userId, string logText)
        {
            var detectionFlowLog = new DetectionFlowLog
            {
                DetectionId = detectionId,
                UserId = userId,
                LogText = logText
            };


            await _detectionFlowLogRepository.SaveOrUpdateAsync(detectionFlowLog);
            await _detectionFlowLogRepository.CommitChangesAsync();
        }


        public async Task<List<DetectionFlowLog>> GetDetectionFlowLog(Guid detectionId)
        {
            var logs = await _detectionFlowLogRepository.QueryAll().Include(x=>x.User).Where(x => x.DetectionId == detectionId).OrderByDescending(x=>x.CreatedAt).ToListAsync();
            return logs;
        }



    }
}
