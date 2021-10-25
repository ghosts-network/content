using GhostEventBus;
using GhostEventBus.Events;
using GhostNetwork.Content.Comments;
using GhostNetwork.Content.Events;
using GhostNetwork.Content.Publications;
using System.Threading.Tasks;

namespace GhostNetwork.Content.Handlers
{
    public class ProfileChangedEventHandler : IEventHandler<ProfileChangedEvent>
    {
        private readonly IPublicationsStorage publicationsStorage;
        private readonly ICommentsStorage commentsStorage;

        public ProfileChangedEventHandler(IPublicationsStorage publicationsStorage, ICommentsStorage commentsStorage)
        {
            this.publicationsStorage = publicationsStorage;
            this.commentsStorage = commentsStorage;
        }

        public async Task Handle(ProfileChangedEvent value)
        {
            await publicationsStorage.UpdateAuthorInfo(value.UpdatedUser.Id, value.UpdatedUser);
            await commentsStorage.UpdateAuthorInfo(value.UpdatedUser.Id, value.UpdatedUser);
        }

        public async Task Handle(EventBase value)
        {
            await Handle(value as ProfileChangedEvent);
        }
    }
}
