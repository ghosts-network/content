using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Domain.Validation;
using GhostNetwork.Content.Api.Helpers.OpenApi;
using GhostNetwork.Content.Comments;
using GhostNetwork.Content.MongoDb;
using GhostNetwork.Content.Publications;
using GhostNetwork.Content.Reactions;
using GhostNetwork.Publications;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Content.Api
{
    public class Startup
    {
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
                    Title = "GhostNetwork.Publications",
                    Description = "Http client for GhostNetwork.Publications",
                    Version = "1.0.0"
                });

                options.OperationFilter<OperationIdFilter>();
                options.OperationFilter<AddResponseHeadersFilter>();

                options.IncludeXmlComments(XmlPathProvider.XmlPath);
            });

            services.AddScoped(_ =>
            {
                var client = new MongoClient($"mongodb://{configuration["MONGO_ADDRESS"]}/gpublications");
                return new MongoDbContext(client.GetDatabase("gpublications"));
            });

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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

        private IValidator<PublicationContext> BuildPublicationValidator(IServiceProvider provider)
        {
            var validators = new List<IValidator<PublicationContext>>
            {
                provider.GetService<ForbiddenWordsValidator>(),
                new MaxLengthValidator(configuration.GetValue<int?>("PUBLICATION_CONTENT_MAX_LENGTH") ?? 5000)
            };

            var minLength = configuration.GetValue<int?>("PUBLICATION_CONTENT_MIN_LENGTH");
            if (minLength.HasValue)
            {
                validators.Add(new MinLengthValidator(minLength.Value));
            }

            return new ValidatorsContainer<PublicationContext>(validators.ToArray());
        }

        private IValidator<CommentContext> BuildCommentValidator(IServiceProvider provider)
        {
            var validators = new List<IValidator<CommentContext>>
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

            return new ValidatorsContainer<CommentContext>(validators.ToArray());
        }
    }
}