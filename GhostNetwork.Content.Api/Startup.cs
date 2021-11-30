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
using GhostNetwork.EventBus;
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

            if (configuration["EVENTHUB_TYPE"]?.ToLower() == "rabbit")
            {
                services.AddSingleton<IEventBus>(provider => new RabbitMqEventBus(
                    new ConnectionFactory { Uri = new Uri(configuration["RABBIT_CONNECTION"]) },
                    new HandlerProvider(provider)));
            }
            else
            {
                services.AddSingleton<IEventBus, NullEventBus>();
            }

            services.AddScoped(_ =>
            {
                // TODO: Remove MONGO_ADDRESS usage after update of all compose files
                var connectionString = configuration["MONGO_CONNECTION"] ??
                                       $"mongodb://{configuration["MONGO_ADDRESS"]}/gpublications";
                var mongoUrl = MongoUrl.Create(connectionString);
                var client = new MongoClient(mongoUrl);
                return new MongoDbContext(client.GetDatabase(mongoUrl.DatabaseName ?? DefaultDbName));
            });

            services.AddScoped<IProfilesApi>(_ => new ProfilesApi(configuration["PROFILES_ADDRESS"]));
            services.AddScoped<IUserProvider, ProfilesApiUserProvider>();

            services.AddScoped<IHashTagsFetcher, DefaultHashTagsFetcher>();
            services.AddScoped(_ => new ForbiddenWordsValidator(Enumerable.Empty<string>()));

            services.AddScoped<IReactionStorage, MongoReactionStorage>();

            services.AddScoped<IPublicationsStorage, MongoPublicationStorage>();
            services.AddScoped<IPublicationService, PublicationService>();
            services.AddScoped(BuildPublicationValidator);

            services.AddScoped<CommentReplyValidator>();
            services.AddScoped<ICommentsStorage, MongoCommentsStorage>();
            services.AddScoped<ICommentsService, CommentsService>();
            services.AddScoped(BuildCommentValidator);

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
                app
                    .UseSwagger()
                    .UseSwaggerUI(config =>
                    {
                        config.SwaggerEndpoint("/swagger/v1/swagger.json", "Relations.API V1");
                    });

                app.UseCors(builder => builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin());
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
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

            var timeLimit = configuration.GetValue<int?>("TIME_LIMIT_TO_UPDATE_PUBLICATIONS");
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

            var timeLimit = configuration.GetValue<int?>("TIME_LIMIT_TO_UPDATE_COMMENTS");
            if (timeLimit.HasValue)
            {
                validators.Add(new TimeLimitToUpdateValidator(TimeSpan.FromSeconds(timeLimit.Value)));
            }

            return new ValidatorsContainer<Comment>(validators.ToArray());
        }
    }
}