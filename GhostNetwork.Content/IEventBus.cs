using System;
using System.Threading.Tasks;

namespace GhostNetwork.Content
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : Event;
    }

    public class NullEventBus : IEventBus
    {
        public Task PublishAsync<TEvent>(TEvent @event) where TEvent : Event
        {
            return Task.CompletedTask;
        }
    }

    public abstract class Event
    {
        protected Event()
        {
            CreatedOn = DateTimeOffset.UtcNow;
            TrackerId = Guid.NewGuid();
        }

        public DateTimeOffset CreatedOn { get; }
        public Guid TrackerId { get; }
    }
}