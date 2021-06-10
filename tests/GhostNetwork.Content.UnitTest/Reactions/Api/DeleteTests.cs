using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GhostNetwork.Content.MongoDb;
using GhostNetwork.Content.Reactions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Reactions.Api
{
    [TestFixture]
    internal class DeleteTests
    {
        [Test]
        public async Task DeleteByAuthor_Ok()
        {
            // Setup
            var key = "key1";
            var author = "some_author";

            var stats = new Dictionary<string, int>
            {
                ["wow"] = 1
            };

            var storageMock = new Mock<IReactionStorage>();

            storageMock
                .Setup(s => s.GetStats(key))
                .ReturnsAsync(stats);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => storageMock.Object);
            });

            var request = new HttpRequestMessage(HttpMethod.Delete, $"reactions/{key}/author");
            request.Headers.Add("author", author);

            // Act
            var response = await client.SendAsync(request);
            var result = await response.Content.DeserializeContent<Dictionary<string, int>>();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(stats["wow"], result["wow"]);
        }

        [Test]
        public async Task DeleteByAuthor_NotFound()
        {
            // Setup
            var key = "key1";
            var author = "some_author";

            var reactions = new Dictionary<string, int>();

            var storageMock = new Mock<IReactionStorage>();
            storageMock
                .Setup(s => s.GetStats(key))
                .ReturnsAsync(reactions);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => storageMock.Object);
            });

            var request = new HttpRequestMessage(HttpMethod.Delete, $"reactions/{key}/author");
            request.Headers.Add("author", author);

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task DeleteByKey_Ok()
        {
            // Setup
            var key = "key1";
            var type = "wow";
            var author = "some_author";

            var stats = new Dictionary<string, int>
            {
                ["key2"] = 1
            };

            var storageMock = new Mock<IReactionStorage>();

            storageMock
                .Setup(s => s.GetStats(key))
                .ReturnsAsync(stats);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => storageMock.Object);
            });

            // Act
            var response = await client.DeleteAsync($"reactions/{key}");
            var result = await response.Content.DeserializeContent<Dictionary<string, int>>();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(stats["key2"], result["key2"]);
        }
    }
}
