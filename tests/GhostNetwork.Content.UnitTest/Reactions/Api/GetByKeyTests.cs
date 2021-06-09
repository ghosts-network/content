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

            var reactions = new ReactionEntity[]
            {
                new ReactionEntity() { Key = key + 1, Type = "some_type1" },
                new ReactionEntity() { Key = key, Type = "some_type2" }
            }.ToDictionary(k => k.Key)
            .GroupBy(r => r.Value)
            .ToDictionary(rg => rg.Key.Key, rg => rg.Count());

            var storageMock = new Mock<IReactionStorage>();

            storageMock
                .Setup(s => s.GetStats(key))
                .ReturnsAsync(reactions);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(provider => storageMock.Object);
            });

            // Act
            var response = await client.GetAsync($"/reactions/{key}");

            var result = await response.Content.DeserializeContent<IDictionary<string, int>>();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(ReactionСomparator.CompareStats(reactions, result));
        }

        [Test]
        public async Task GetByKey_NotFound()
        {
            // Setup
            var key = "non_existent_key";

            var reactions = new ReactionEntity[] { }
                .ToDictionary(k => k.Key)
                .GroupBy(r => r.Value)
                .ToDictionary(rg => rg.Key.Key, rg => rg.Count());

            var storageMock = new Mock<IReactionStorage>();

            storageMock
                .Setup(s => s.GetStats(key))
                .ReturnsAsync(reactions);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(provider => storageMock.Object);
            });

            // Act
            var response = await client.GetAsync($"/reactions/{key}");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}