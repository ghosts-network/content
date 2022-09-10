using System;
using GhostNetwork.EventBus;

namespace GhostNetwork.Content.Publications;

public record CreatedEvent(string Id, string Content, DateTimeOffset CreatedOn, DateTimeOffset UpdatedOn, UserInfo Author) : Event;
