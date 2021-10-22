namespace GhostNetwork.Content.EventBus.RabbitMq
{
    public interface IQueueNameProvider
    {
        string GetName<TEvent>(TEvent @event) where TEvent : Event;
    }
}