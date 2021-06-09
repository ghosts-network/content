using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
                collection.AddScoped(provider => storageMock.Object);
            });

            var request = new HttpRequestMessage(HttpMethod.Post, $"reactions/{key}/{type}");
            request.Headers.Add("author", author);

            // Act
            var response = await client.SendAsync(request);
            var result = await response.Content.DeserializeContent<Dictionary<string, int>>();

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsTrue(ReactionСomparator.CompareStats(stats, result));
        }

        [Test]
        public async Task UpsertReaction_Created()
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
                collection.AddScoped(provider => storageMock.Object);
            });

            var firstRequest = new HttpRequestMessage(HttpMethod.Post, $"reactions/{key}/{type}");
            firstRequest.Headers.Add("author", author);

            var secondRequest = new HttpRequestMessage(HttpMethod.Post, $"reactions/{key}/like");
            secondRequest.Headers.Add("author", author);

            // Act
            var firstResponse = await client.SendAsync(firstRequest);
            var firstResult = await firstResponse.Content.DeserializeContent<Dictionary<string, int>>();

            var updatedStats = new Dictionary<string, int>
            {
                { "like", 1 }
            };

            storageMock
                .Setup(s => s.GetStats(key))
                .ReturnsAsync(updatedStats);

            var secondResponse = await client.SendAsync(secondRequest);
            var secondResult = await secondResponse.Content.DeserializeContent<Dictionary<string, int>>();

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, firstResponse.StatusCode);
            Assert.AreEqual(HttpStatusCode.Created, secondResponse.StatusCode);
            Assert.IsFalse(ReactionСomparator.CompareStats(firstResult, secondResult));
        }
    }
}
