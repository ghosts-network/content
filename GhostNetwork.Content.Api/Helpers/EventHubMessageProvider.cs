using System;
using System.Linq;
using Azure.Messaging.ServiceBus;
using GhostNetwork.EventBus;
using GhostNetwork.EventBus.AzureServiceBus;
using Microsoft.AspNetCore.Http;

namespace GhostNetwork.Content.Api.Helpers;

public class EventHubMessageProvider : IMessageProvider
{
    private readonly JsonMessageProvider jsonMessageProvider = new JsonMessageProvider();
    private readonly IHttpContextAccessor httpContextAccessor;

    public EventHubMessageProvider(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public ServiceBusMessage GetMessage<TEvent>(TEvent @event)
        where TEvent : Event
    {
        var message = jsonMessageProvider.GetMessage(@event);
        message.CorrelationId = httpContextAccessor.HttpContext!.Request.Headers["X-Request-ID"].FirstOrDefault() ??
                                Guid.NewGuid().ToString();

        return message;
    }

    public object GetEvent(byte[] message, Type outputType)
    {
        return jsonMessageProvider.GetEvent(message, outputType);
    }
}