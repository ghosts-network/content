using GhostNetwork.EventBus;

namespace GhostNetwork.Content.Publications;

public record DeletedEvent(string Id, UserInfo Author) : Event;
