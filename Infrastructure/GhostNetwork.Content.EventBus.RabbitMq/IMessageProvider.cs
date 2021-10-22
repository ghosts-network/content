using System.Text;
using System.Text.Json;

namespace GhostNetwork.Content.EventBus.RabbitMq
{
    public interface IMessageProvider
    {
        byte[] GetMessage<TEvent>(TEvent @event) where TEvent : Event;
    }

    public class JsonMessageProvider : IMessageProvider
    {
        public byte[] GetMessage<TEvent>(TEvent @event) where TEvent : Event
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
        }
    }
}