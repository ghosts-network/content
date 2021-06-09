using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Content.Api.Models;
using GhostNetwork.Content.Reactions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Reactions.Api
{
    [TestFixture]
    internal class GetGroupedTest
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
            var response = await client.PostAsync("reactions/grouped", input.AsJsonContent());
            var result = await response.Content.DeserializeContent<Dictionary<string, Dictionary<string, int>>>();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(result["Post_Test1"]["wow"] == 2 && result["Post_Test1"]["like"] == 1 && result["Post_Test2"]["like"] == 1);
        }
    }
}