namespace GhostNetwork.Content.EventBus.RabbitMq
{
    public class DefaultQueueNameProvider : IQueueNameProvider
    {
        public string GetName<TEvent>(TEvent @event) where TEvent : Event
        {
            var name = @event.GetType().FullName!.ToLower();
            if (name.EndsWith("event"))
            {
                name = name[..^5];
            }

            return name;
        }
    }
}