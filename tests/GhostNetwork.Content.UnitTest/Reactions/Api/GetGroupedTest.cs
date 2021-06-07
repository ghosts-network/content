using Moq;
using System.Net;
using NUnit.Framework;
using System.Threading.Tasks;
using GhostNetwork.Content.Reactions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Newtonsoft.Json;
using GhostNetwork.Content.Api.Models;

namespace GhostNetwork.Content.UnitTest.Reactions.Api
{
    [TestFixture]
    public class GetGroupedTest
    {
        [Test]
        public async Task GetGrouppedReactions_Ok()
        {
            // Setup
            var keys = new string[] { "Post_Test1", "Post_Test2" };

            var data = new Dictionary<string, Dictionary<string, int>>
            {
                { "Post_Test1", new Dictionary<string, int>() { ["like"] = 1, ["wow"] = 2 } },
                { "Post_Test2", new Dictionary<string, int>() { ["like"] = 1 } }
            };

            var storageMock = new Mock<IReactionStorage>();

            storageMock
                .Setup(s => s.GetGroupedReactionsAsync(keys))
                .ReturnsAsync(data);

            var client = TestServerHelper.New(collection => 
            {
                collection.AddScoped(provider => storageMock.Object);
            });

            var input = new ReactionsQuery { Keys = keys };

            // Act
            var response = await client.PostAsync("reactions/grouped", input.AsJsonContent<ReactionsQuery>());

            var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(
                await response.Content.ReadAsStringAsync());
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(result["Post_Test1"]["wow"] == 2 && result["Post_Test1"]["like"] == 1 && result["Post_Test2"]["like"] == 1);
        }
    }
}