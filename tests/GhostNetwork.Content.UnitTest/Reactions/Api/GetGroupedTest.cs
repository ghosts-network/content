using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Content.Api.Models;
using GhostNetwork.Content.Reactions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Reactions.Api
{
    [TestFixture]
    internal class GetGroupedTest
    {
        [Test]
        public async Task GetGroupedReactions_Ok()
        {
            // Setup
            var data = new Dictionary<string, Dictionary<string, int>>
            {
                { "Post_Test1", new Dictionary<string, int> { ["like"] = 1, ["wow"] = 2 } },
                { "Post_Test2", new Dictionary<string, int> { ["like"] = 1 } }
            };
            var keys = data.Select(d => d.Key).ToList();

            var storageMock = new Mock<IReactionStorage>();
            storageMock
                .Setup(s => s.GetGroupedReactionsAsync(keys))
                .ReturnsAsync(data);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => storageMock.Object);
            });

            var input = new ReactionsQuery { Keys = keys };

            // Act
            var response = await client.PostAsync("reactions/grouped", input.AsJsonContent());
            var result = await response.Content.DeserializeContent<Dictionary<string, Dictionary<string, int>>>();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(2, result["Post_Test1"]["wow"]);
            Assert.AreEqual(1, result["Post_Test1"]["like"]);
            Assert.AreEqual(1, result["Post_Test2"]["like"]);
        }
    }
}