using System.Threading.Tasks;
using RabbitMQ.Client;

namespace GhostNetwork.Content.EventBus.RabbitMq
{
    public class RabbitMqEventBus : IEventBus
    {
        private readonly ConnectionProvider connectionProvider;
        private readonly IMessageProvider messageProvider;
        private readonly IQueueNameProvider queueNameGenerator;

        public RabbitMqEventBus(ConnectionFactory connectionFactory, IMessageProvider messageProvider = null, IQueueNameProvider queueNameGenerator = null)
        {
            connectionProvider = new ConnectionProvider(connectionFactory);
            this.messageProvider = messageProvider ?? new JsonMessageProvider();
            this.queueNameGenerator = queueNameGenerator ?? new DefaultQueueNameProvider();
        }

        public Task PublishAsync<TEvent>(TEvent @event) where TEvent : Event
        {
            using var channel = connectionProvider.GetConnection().CreateModel();

            var queueName = queueNameGenerator.GetName(@event);
            channel.ExchangeDeclare(queueName, ExchangeType.Fanout);
            channel.BasicPublish(queueName, "", null, messageProvider.GetMessage(@event));

            channel.Close();
            return Task.CompletedTask;
        }
    }
}
