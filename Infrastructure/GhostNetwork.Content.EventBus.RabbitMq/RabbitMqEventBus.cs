using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GhostNetwork.Content.EventBus.RabbitMq
{
    public class RabbitMqEventBus : IEventBus, IDisposable
    {
        private readonly ConnectionProvider connectionProvider;
        private readonly IMessageProvider messageProvider;
        private readonly INameProvider nameProvider;
        private readonly ConcurrentDictionary<string, IModel> subscriptions = new();

        public RabbitMqEventBus(ConnectionFactory connectionFactory, IMessageProvider messageProvider = null, INameProvider nameProvider = null)
        {
            connectionProvider = new ConnectionProvider(connectionFactory);
            this.messageProvider = messageProvider ?? new JsonMessageProvider();
            this.nameProvider = nameProvider ?? new DefaultQueueNameProvider();
        }

        public Task PublishAsync<TEvent>(TEvent @event) where TEvent : Event
        {
            using var channel = connectionProvider.GetConnection().CreateModel();

            var exchangeName = nameProvider.GetExchangeName<TEvent>();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
            channel.BasicPublish(exchangeName, "", null, messageProvider.GetMessage(@event));

            channel.Close();
            return Task.CompletedTask;
        }

        public void Subscribe<TEvent, THandler>() where THandler : IEventHandler<TEvent> where TEvent : Event
        {
            var channel = connectionProvider.GetConnection().CreateModel();

            var exchangeName = nameProvider.GetExchangeName<TEvent>();
            var queueName = nameProvider.GetQueueName<TEvent, THandler>();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
            channel.QueueDeclare(queueName);
            channel.QueueBind(queueName, exchangeName, "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (_, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("[x] {0}", message);
            };

            channel.BasicConsume(queueName, true, consumer);
            subscriptions.TryAdd(nameProvider.GetSubscriptionName<TEvent, THandler>(), channel);
        }

        public void Unsubscribe<TEvent, THandler>() where TEvent : Event where THandler : IEventHandler<TEvent>
        {
            subscriptions.TryRemove(nameProvider.GetSubscriptionName<TEvent, THandler>(), out _);
        }

        public void Dispose()
        {
            connectionProvider?.Dispose();
        }
    }
}
