using System.Threading.Tasks;
using GhostNetwork.Content.Comments;
using GhostNetwork.Content.Publications;
using GhostNetwork.EventBus;

namespace GhostNetwork.Content;

public class ProfileUpdatedHandler : IEventHandler<GhostNetwork.Profiles.UpdatedEvent>
{
    private readonly IPublicationsStorage publicationsStorage;
    private readonly ICommentsStorage commentsStorage;

    public ProfileUpdatedHandler(IPublicationsStorage publicationsStorage,
        ICommentsStorage commentsStorage)
    {
        this.publicationsStorage = publicationsStorage;
        this.commentsStorage = commentsStorage;
    }

    public async Task ProcessAsync(GhostNetwork.Profiles.UpdatedEvent @event)
    {
        await publicationsStorage.UpdateAuthorAsync(@event.Id, @event.FullName, @event.ProfilePicture);
        await commentsStorage.UpdateAuthorAsync(@event.Id, @event.FullName, @event.ProfilePicture);
    }
}