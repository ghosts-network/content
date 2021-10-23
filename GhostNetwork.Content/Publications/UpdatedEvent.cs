namespace GhostNetwork.Content.Publications
{
    public record UpdatedEvent(string Id, string Content, UserInfo Author) : TrackableEvent;
}