using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GhostNetwork.Content.Reactions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Reactions.Api
{
    [TestFixture]
    internal class UpsertTests
    {
        [Test]
        public async Task PostReaction_Created()
        {
            // Setup
            var author = "some_author";
            var key = "some_key";
            var type = "wow";

            var stats = new Dictionary<string, int>
            {
                { type, 1 }
            };

            var storageMock = new Mock<IReactionStorage>();
            storageMock
                .Setup(s => s.GetStats(key))
                .ReturnsAsync(stats);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => storageMock.Object);
            });

            var request = new HttpRequestMessage(HttpMethod.Post, $"reactions/{key}/{type}");
            request.Headers.Add("author", author);

            // Act
            var response = await client.SendAsync(request);
            var result = await response.Content.DeserializeContent<Dictionary<string, int>>();

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(stats[type], result[type]);
        }
    }
}
