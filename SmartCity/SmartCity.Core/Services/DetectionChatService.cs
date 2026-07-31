using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels.DetectionChat;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace SmartCity.Core.Services;

public class DetectionChatService : ServiceBase, IDetectionChatService
{
    private readonly IGenericRepositorySimpleUniqueIdentifier<DetectionChat> _detectionChatRepository;
    private readonly IApplicationContext<User> _applicationContext;

    public DetectionChatService(IApplicationContext<User> applicationContext, IGenericRepositorySimpleUniqueIdentifier<DetectionChat> detectionChatRepository) : base(applicationContext)
    {
        _detectionChatRepository = detectionChatRepository;
        _applicationContext = applicationContext;
    }

    public async Task AddDetectionChatAsync(DetectionChatIn data)
    {
        var detectionChat = new DetectionChat
        {
            CreatedAt = DateTime.UtcNow,
            Text = data.Text,
            DetectionId = data.DetectionId,
            UserId = _applicationContext.SecurityContext.UserId,
        };

        await _detectionChatRepository.SaveOrUpdateAsync(detectionChat);
        await _detectionChatRepository.CommitChangesAsync();
    }

    public async Task<List<DetectionChatOut>> GetChatByDetectionIdAsync(Guid detectionId)
    {
        var detectionChat = await _detectionChatRepository.QueryAll().Where(x => x.DetectionId == detectionId).OrderByDescending(x=>x.CreatedAt).Select(x => new DetectionChatOut
        {
            DetectionId = x.DetectionId,
            Text = x.Text,
            CreatedAt = x.CreatedAt,
            AddedBy = x.User.FullName,
        }).ToListAsync();

        return detectionChat;
    }
}
