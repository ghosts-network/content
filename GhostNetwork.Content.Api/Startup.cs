using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Domain.Validation;
using GhostNetwork.Content.Api.Helpers;
using GhostNetwork.Content.Api.Helpers.OpenApi;
using GhostNetwork.Content.Comments;
using GhostNetwork.Content.MongoDb;
using GhostNetwork.Content.Publications;
using GhostNetwork.Content.Reactions;
using GhostNetwork.Content.Redis;
using GhostNetwork.EventBus;
using GhostNetwork.EventBus.AzureServiceBus;
using GhostNetwork.EventBus.RabbitMq;
using GhostNetwork.Profiles.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using RabbitMQ.Client;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Content.Api
{
    public class Startup
    {
        private const string DefaultDbName = "profiles";

        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            if (configuration.GetValue<bool>("OPENAPI_JSON_ENABLED"))
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "GhostNetwork.Content",
                        Description = "Http client for GhostNetwork.Content",
                        Version = "1.0.0"
                    });

                    options.OperationFilter<OperationIdFilter>();
                    options.OperationFilter<AddResponseHeadersFilter>();

                    options.IncludeXmlComments(XmlPathProvider.XmlPath);
                });
            }

            switch (configuration["EVENTHUB_TYPE"]?.ToLower())
            {
                case "rabbit":
                    services.AddSingleton<IEventBus>(provider => new RabbitMqEventBus(
                        new ConnectionFactory { Uri = new Uri(configuration["RABBIT_CONNECTION"]) },
                        new EventBus.RabbitMq.HandlerProvider(provider)));
                    break;
                case "servicebus":
                    services.AddSingleton<IEventBus>(provider => new AzureServiceEventBus(
                        configuration["SERVICEBUS_CONNECTION"],
                        new EventBus.AzureServiceBus.HandlerProvider(provider)));
                    break;
                default:
                    services.AddSingleton<IEventBus, NullEventBus>();
                    break;
            }

            services.AddScoped(_ =>
            {
                var connectionString = configuration["MONGO_CONNECTION"];
                var mongoUrl = MongoUrl.Create(connectionString);
                var client = new MongoClient(mongoUrl);
                return new MongoDbContext(client.GetDatabase(mongoUrl.DatabaseName ?? DefaultDbName));
            });

            services.AddSingleton(ConnectionMultiplexer.Connect(configuration["REDIS_CONNECTION"]));
            services.AddScoped(provider =>
            {
                var redisConnection = provider.GetRequiredService<ConnectionMultiplexer>();

                return redisConnection.GetDatabase();
            });

            services.AddScoped<IProfilesApi>(_ => new ProfilesApi(configuration["PROFILES_ADDRESS"]));
            services.AddScoped<IUserProvider, ProfilesApiUserProvider>();

            services.AddScoped<IHashTagsFetcher, DefaultHashTagsFetcher>();
            services.AddScoped(_ => new ForbiddenWordsValidator(Enumerable.Empty<string>()));

            switch (configuration["REACTION_STORAGE_TYPE"])
            {
                case "redis":
                    services.AddScoped<IReactionStorage, RedisReactionStorage>();
                    break;
                default:
                    services.AddScoped<IReactionStorage, MongoReactionStorage>();
                    break;
            }

            services.AddScoped<IPublicationsStorage, MongoPublicationStorage>();
            services.AddScoped<IPublicationService, PublicationService>();
            services.AddScoped(BuildPublicationValidator);

            services.AddScoped<CommentReplyValidator>();
            services.AddScoped<ICommentsStorage, MongoCommentsStorage>();
            services.AddScoped<ICommentsService, CommentsService>();
            services.AddScoped(BuildCommentValidator);
            services.AddScoped<ProfileUpdatedHandler>();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime hostApplicationLifetime)
        {
            if (env.IsDevelopment())
            {
                bool openApiEnabled = configuration.GetValue<bool>("OPENAPI_JSON_ENABLED");
                bool swaggerUiEnabled = configuration.GetValue<bool>("SWAGGER_UI_ENABLED");

                if (openApiEnabled)
                {
                    app.UseSwagger();
                }

                if (openApiEnabled && swaggerUiEnabled)
                {
                    app.UseSwaggerUI(config =>
                    {
                        config.SwaggerEndpoint("/swagger/v1/swagger.json", "Relations.API V1");
                    });
                }

                app.UseCors(builder => builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin());
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
                eventBus.Subscribe<Profiles.UpdatedEvent, ProfileUpdatedHandler>();
            });

            hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                var scope = app.ApplicationServices.CreateScope();
                var mongoDb = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
                mongoDb.ConfigureAsync().GetAwaiter().GetResult();
            });
        }

        private IValidator<Publication> BuildPublicationValidator(IServiceProvider provider)
        {
            var validators = new List<IValidator<Publication>>
            {
                provider.GetService<ForbiddenWordsValidator>(),
                new MaxLengthValidator(configuration.GetValue<int?>("PUBLICATION_CONTENT_MAX_LENGTH") ?? 5000)
            };

            var minLength = configuration.GetValue<int?>("PUBLICATION_CONTENT_MIN_LENGTH");
            if (minLength.HasValue)
            {
                validators.Add(new MinLengthValidator(minLength.Value));
            }

            var timeLimit = configuration.GetValue<int?>("PUBLICATION_UPDATE_TIME_LIMIT");
            if (timeLimit.HasValue)
            {
                validators.Add(new TimeLimitToUpdateValidator(TimeSpan.FromSeconds(timeLimit.Value)));
            }

            return new ValidatorsContainer<Publication>(validators.ToArray());
        }

        private IValidator<Comment> BuildCommentValidator(IServiceProvider provider)
        {
            var validators = new List<IValidator<Comment>>
            {
                provider.GetService<CommentReplyValidator>(),
                provider.GetService<ForbiddenWordsValidator>(),
                new MaxLengthValidator(configuration.GetValue<int?>("COMMENT_CONTENT_MAX_LENGTH") ?? 5000)
            };

            var minLength = configuration.GetValue<int?>("COMMENT_CONTENT_MIN_LENGTH");
            if (minLength.HasValue)
            {
                validators.Add(new MinLengthValidator(minLength.Value));
            }

            var timeLimit = configuration.GetValue<int?>("COMMENT_UPDATE_TIME_LIMIT");
            if (timeLimit.HasValue)
            {
                validators.Add(new TimeLimitToUpdateValidator(TimeSpan.FromSeconds(timeLimit.Value)));
            }

            return new ValidatorsContainer<Comment>(validators.ToArray());
        }
    }
}