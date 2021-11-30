using System;
using System.Threading.Tasks;
using GhostNetwork.EventBus;

namespace GhostNetwork.Profiles
{
    public record UpdatedEvent(Guid Id, string FullName, string ProfilePicture) : TrackableEvent;

    public class ProfileUpdatedHandler : IEventHandler<UpdatedEvent>
    {
        public async Task ProcessAsync(UpdatedEvent @event)
        {
            
        }
    }
}
