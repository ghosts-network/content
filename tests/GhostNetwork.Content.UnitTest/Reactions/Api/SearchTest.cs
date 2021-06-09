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
    internal class SearchTest
    {
        [Test]
        public async Task SearchTest_Ok()
        {
            var author = "some_author";
            var keys = new string[] { "key1", "key2" };

            var reactions = new Reaction[]
            {
                new Reaction(keys[0], "like"),
                new Reaction(keys[1], "wow")
            };

            var storageMock = new Mock<IReactionStorage>();

            storageMock
                .Setup(s => s.GetReactionsByAuthorAsync(author, keys))
                .ReturnsAsync(reactions);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(provider => storageMock.Object);
            });

            var input = new ReactionsQuery() { Keys = keys };

            // Act
            var response = await client.PostAsync($"reactions/search?author={author}", input.AsJsonContent());
            var result = await response.Content.DeserializeContent<Reaction[]>();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(ReactionСomparator.Compare(reactions, result));
        }
    }
}