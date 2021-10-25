using System;
using System.Collections.Concurrent;
using RabbitMQ.Client;

namespace GhostNetwork.Content.EventBus.RabbitMq
{
    public class SubscriptionManager
    {
        private readonly ConcurrentDictionary<string, Subscription> subscriptions = new();

        public void Subscribe<TEvent, THandler>(IModel model)
        {
            subscriptions.TryAdd(GetSubscriptionName<TEvent, THandler>(), new Subscription(typeof(TEvent), typeof(THandler), model));
        }

        public void Unsubscribe<TEvent, THandler>()
        {
            subscriptions.TryRemove(GetSubscriptionName<TEvent, THandler>(), out _);
        }

        private static string GetSubscriptionName<TEvent, THandler>() =>
            $"{typeof(TEvent).FullName}-{typeof(THandler).FullName}";
        private record Subscription(Type Event, Type Handler, IModel Channel);
    }

    public interface IHandlerProvider
    {
        object GetRequiredService(Type type);
    }
}