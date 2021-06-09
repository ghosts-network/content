using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GhostNetwork.Content.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace GhostNetwork.Content.UnitTest
{
    public static class TestServerHelper
    {
        public static HttpClient New(Action<IServiceCollection> configureServices)
        {
            var server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureTestServices(configureServices));

            return server.CreateClient();
        }

        public static StringContent AsJsonContent<T>(this T input)
        {
            return new StringContent(JsonConvert.SerializeObject(input), Encoding.Default, "application/json");
        }

        public static async Task<T> DeserializeContent<T>(this HttpContent content)
        {
            return JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync());
        }
    }
}