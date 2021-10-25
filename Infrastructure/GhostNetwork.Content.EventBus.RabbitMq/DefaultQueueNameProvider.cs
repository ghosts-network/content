namespace GhostNetwork.Content.EventBus.RabbitMq
{
    public class DefaultQueueNameProvider : INameProvider
    {
        public string GetSubscriptionName<TEvent, THandler>() where TEvent : Event where THandler : IEventHandler<TEvent>
        {
            return $"{GetExchangeName<TEvent>()}-{GetQueueName<TEvent, THandler>()}";
        }

        public string GetQueueName<TEvent, THandler>() where TEvent : Event where THandler : IEventHandler<TEvent>
        {
            return typeof(THandler).FullName!.ToLower();
        }

        public string GetExchangeName<TEvent>() where TEvent : Event
        {
            var name = typeof(TEvent).FullName!.ToLower();
            if (name.EndsWith("event"))
            {
                name = name[..^5];
            }

            return name;
        }
    }
}