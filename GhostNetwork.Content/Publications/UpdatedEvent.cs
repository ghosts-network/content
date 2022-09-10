using System;
using GhostNetwork.EventBus;

namespace GhostNetwork.Content.Publications;

public record UpdatedEvent(string Id, string Content, DateTimeOffset CreatedOn, DateTimeOffset UpdatedOn, UserInfo Author) : Event;
