using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace GhostNetwork.Content.EventBus.RabbitMq
{
    public class RabbitMqEventBus : IEventBus
    {
        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : Event
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare(@event.GetType().FullName, ExchangeType.Fanout);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
            channel.BasicPublish(@event.GetType().FullName, "", null, body);
        }
    }
}