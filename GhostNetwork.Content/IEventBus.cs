using System;
using System.Threading.Tasks;

namespace GhostNetwork.Content
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : Event;
        void Subscribe<TEvent, THandler>()
            where TEvent : Event
            where THandler : IEventHandler<TEvent>;
        void Unsubscribe<TEvent, THandler>()
            where TEvent : Event
            where THandler : IEventHandler<TEvent>;
    }

    public interface IEventHandler<in TEvent> where TEvent : class
    {
        Task ProcessAsync(TEvent @event);
    }

    public class NullEventBus : IEventBus
    {
        public Task PublishAsync<TEvent>(TEvent @event) where TEvent : Event
        {
            return Task.CompletedTask;
        }

        public void Subscribe<TEvent, THandler>()
            where TEvent : Event
            where THandler : IEventHandler<TEvent>
        { }

        public void Unsubscribe<TEvent, THandler>() where TEvent : Event where THandler : IEventHandler<TEvent>
        { }
    }

    public abstract record TrackableEvent : Event
    {
        protected TrackableEvent()
        {
            CreatedOn = DateTimeOffset.UtcNow;
            TrackerId = Guid.NewGuid();
        }

        public DateTimeOffset CreatedOn { get; }
        public Guid TrackerId { get; }
    }

    public abstract record Event;
}