using GhostNetwork.EventBus.RabbitMq;
using Microsoft.AspNetCore.Http;
using RabbitMQ.Client;

namespace GhostNetwork.Content.Api.Helpers;

public class RabbitMqPropertiesProvider : EmptyPropertiesProvider
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public RabbitMqPropertiesProvider(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public IBasicProperties GetProperties(IModel channel)
    {
        var properties = base.GetProperties(channel);
        properties.CorrelationId = httpContextAccessor.HttpContext?.TraceIdentifier;

        return properties;
    }
}