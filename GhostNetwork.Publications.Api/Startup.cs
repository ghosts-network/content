using GhostNetwork.Publications.Domain;
using GhostNetwork.Publications.MongoDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

namespace GhostNetwork.Publications.Api
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
                    Title = "GhostNetwork/Publications API",
                    Version = "1.0.0"
                });
            });

            services.AddScoped<IHashTagsFetcher, DefaultHashTagsFetcher>();
            services.AddScoped<PublicationBuilder>();
            services.AddScoped<IPublicationStorage, MongoPublicationStorage>(provider =>
            {
                var client = new MongoClient($"mongodb://{configuration["MONGO_ADDRESS"]}/gnpublications");
                var context = new MongoDbContext(client.GetDatabase("gnpublications"));
                return new MongoPublicationStorage(context);
            });
            services.AddControllers();
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
    }
}