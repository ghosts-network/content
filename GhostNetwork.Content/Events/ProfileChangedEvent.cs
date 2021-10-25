using GhostEventBus.Events;

namespace GhostNetwork.Content.Events
{
    public class ProfileChangedEvent : EventBase
    {
        public UserInfo UpdatedUser { get; init; }
    }
}