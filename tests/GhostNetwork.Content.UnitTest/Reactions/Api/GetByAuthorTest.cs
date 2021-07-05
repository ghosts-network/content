using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GhostNetwork.Content.Reactions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Reactions.Api
{
    [TestFixture]
    internal class GetByAuthorTest
    {
        [Test]
        public async Task GetByAuthor_Ok()
        {
            // Setup
            var authorKey = "some_author";
            var reactionKey = "some_reaction";
            var type = "some_type";

            var reaction = new Reaction(reactionKey, type);

            var storageMock = new Mock<IReactionStorage>();
            storageMock
                .Setup(s => s.GetReactionByAuthorAsync(reactionKey, authorKey))
                .ReturnsAsync(reaction);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => storageMock.Object);
            });

            var request = new HttpRequestMessage(HttpMethod.Get, $"reactions/{reactionKey}/author");
            request.Headers.Add("author", authorKey);

            // Act
            var response = await client.SendAsync(request);

            var result = JsonConvert.DeserializeObject<Reaction>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(reaction.Key, result.Key);
            Assert.AreEqual(reaction.Type, result.Type);
        }

        [Test]
        public async Task GetByAuthor_NotFound()
        {
            var authorKey = "non_exist_author";
            var reactionKey = "non_exist_reaction";

            var storageMock = new Mock<IReactionStorage>();
            storageMock
                .Setup(s => s.GetReactionByAuthorAsync(reactionKey, authorKey))
                .ReturnsAsync(default(Reaction));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => storageMock.Object);
            });

            var request = new HttpRequestMessage(HttpMethod.Get, $"reactions/{reactionKey}/author");
            request.Headers.Add("author", authorKey);

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}