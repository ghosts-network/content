using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Content.MongoDb;
using GhostNetwork.Content.Reactions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Reactions.Api
{
    [TestFixture]
    internal class GetByKeyTests
    {
        [Test]
        public async Task GetByKey_Ok()
        {
            // Setup
            var key = "some_key";
            var stats = new Dictionary<string, int>()
            {
                ["like"] = 2,
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

            // Act
            var response = await client.GetAsync($"/reactions/{key}");
            var result = await response.Content.DeserializeContent<IDictionary<string, int>>();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(2, result["like"]);
            Assert.AreEqual(1, result["wow"]);
        }

        [Test]
        public async Task GetByKey_NotFound()
        {
            // Setup
            var key = "non_existent_key";

            var stats = new Dictionary<string, int>();

            var storageMock = new Mock<IReactionStorage>();
            storageMock
                .Setup(s => s.GetStats(key))
                .ReturnsAsync(stats);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => storageMock.Object);
            });

            // Act
            var response = await client.GetAsync($"/reactions/{key}");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}