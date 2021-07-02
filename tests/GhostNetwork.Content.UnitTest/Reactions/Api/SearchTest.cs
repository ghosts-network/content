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
    internal class SearchTest
    {
        [Test]
        public async Task SearchTest_Ok()
        {
            var author = "some_author";
            var reactions = new[]
            {
                new Reaction("key1", "like"),
                new Reaction("key2", "wow")
            };
            var keys = reactions.Select(r => r.Key).ToList();

            var storageMock = new Mock<IReactionStorage>();

            storageMock
                .Setup(s => s.GetReactionsByAuthorAsync(author, keys))
                .ReturnsAsync(reactions);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => storageMock.Object);
            });

            var input = new ReactionsQuery { Keys = keys };

            // Act
            var response = await client.PostAsync($"reactions/search?author={author}", input.AsJsonContent());
            var result = await response.Content.DeserializeContent<ReactionDto[]>();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(new ReactionDto(reactions[0].Key, reactions[0].Type), result[0]);
            Assert.AreEqual(new ReactionDto(reactions[1].Key, reactions[1].Type), result[1]);
        }
    }

    public record ReactionDto(string key, string type);
}