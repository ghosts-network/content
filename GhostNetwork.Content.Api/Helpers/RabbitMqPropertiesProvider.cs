using System;
using System.Linq;
using GhostNetwork.EventBus.RabbitMq;
using Microsoft.AspNetCore.Http;
using RabbitMQ.Client;

namespace GhostNetwork.Content.Api.Helpers;

public class RabbitMqPropertiesProvider : IPropertiesProvider
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public RabbitMqPropertiesProvider(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public IBasicProperties GetProperties(IModel channel)
    {
        var properties = channel.CreateBasicProperties();
        properties.CorrelationId = httpContextAccessor.HttpContext!.Request.Headers["X-Request-ID"].FirstOrDefault() ??
                                   Guid.NewGuid().ToString();

        return properties;
    }
}