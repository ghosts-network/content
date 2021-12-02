using System;
using System.Threading.Tasks;
using GhostNetwork.Content.Comments;
using GhostNetwork.Content.Publications;
using GhostNetwork.EventBus;

// ReSharper disable once CheckNamespace
namespace GhostNetwork.Profiles
{
    public record UpdatedEvent(Guid Id, string FullName, string ProfilePicture) : TrackableEvent;

    public class ProfileUpdatedHandler : IEventHandler<UpdatedEvent>
    {
        private readonly IPublicationsStorage publicationsStorage;
        private readonly ICommentsStorage commentsStorage;

        public ProfileUpdatedHandler(IPublicationsStorage publicationsStorage,
            ICommentsStorage commentsStorage)
        {
            this.publicationsStorage = publicationsStorage;
            this.commentsStorage = commentsStorage;
        }

        public async Task ProcessAsync(UpdatedEvent @event)
        {
            await publicationsStorage.UpdateAuthorAsync(@event.Id, @event.FullName, @event.ProfilePicture);
            await commentsStorage.UpdateAuthorAsync(@event.Id, @event.FullName, @event.ProfilePicture);
        }
    }
}
