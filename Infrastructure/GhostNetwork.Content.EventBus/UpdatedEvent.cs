using System;
using GhostNetwork.EventBus;

// ReSharper disable once CheckNamespace
namespace GhostNetwork.Profiles;

public record UpdatedEvent(Guid Id, string FullName, string ProfilePicture) : TrackableEvent;