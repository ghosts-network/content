using GhostNetwork.EventBus;

namespace GhostNetwork.Content.Publications
{
    public record CreatedEvent(string Id, string Content, UserInfo Author) : TrackableEvent;
}