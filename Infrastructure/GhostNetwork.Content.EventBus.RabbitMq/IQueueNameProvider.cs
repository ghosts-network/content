namespace GhostNetwork.Content.EventBus.RabbitMq
{
    public interface INameProvider
    {
        string GetQueueName<TEvent, THandler>()
            where TEvent : Event
            where THandler : IEventHandler<TEvent>;
        string GetExchangeName<TEvent>()
            where TEvent : Event;
    }
}