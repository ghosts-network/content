using GhostNetwork.Content.Api.Models;
using GhostNetwork.Content.Comments;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GhostNetwork.Content.UnitTest.Comments.Api
{
    [TestFixture]
    public class SearchTests
    {
        [Test]
        public async Task Search_Ok()
        {
            // Setup
            const string publicationId = "someId";
            const int skip = 0;
            const int take = 5;
            const string cursor = null;
            const Ordering order = Ordering.Asc;

            var commentId = 1;

            var author = new UserInfo(Guid.NewGuid(), "SomeName", null);

            var comments = new Comment[]
            {
                new Comment((commentId++).ToString(), "someContent1", DateTimeOffset.Now, publicationId, null, author),
                new Comment((commentId++).ToString(), "someContent2", DateTimeOffset.Now, publicationId, null, author),
                new Comment((commentId++).ToString(), "someContent3", DateTimeOffset.Now, publicationId, null, author)
            };

            var serviceMock = new Mock<ICommentsService>();
            serviceMock.Setup(s => s.SearchAsync(publicationId, skip, take, cursor, order)).ReturnsAsync((comments, comments.Count()));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.GetAsync($"comments/bykey/{publicationId}?skip={skip}&take={take}");
            var result = await response.Content.DeserializeContent<IEnumerable<Comment>>();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(result.Count(), 3);
        }

        [Test]
        public async Task SearchFeatured_Ok()
        {
            // Setup
            const string firstPublicationId = "firstId";
            const string secondPublicationId = "secondId";

            int commentId = 1;

            var author = new UserInfo(Guid.NewGuid(), "SomeName", null);

            var featuredComments = new Dictionary<string, FeaturedInfo>()
            {
                {
                    firstPublicationId, new FeaturedInfo(
                        new Comment[]
                        {
                            new Comment((commentId++).ToString(), "someContent1", DateTimeOffset.Now, firstPublicationId, null, author),
                            new Comment((commentId++).ToString(), "someContent2", DateTimeOffset.Now, firstPublicationId, null, author),
                        }, 2)
                },
                {
                    secondPublicationId, new FeaturedInfo(
                        new Comment[]
                        {
                            new Comment((commentId++).ToString(), "someContent1", DateTimeOffset.Now, secondPublicationId, null, author),
                            new Comment((commentId++).ToString(), "someContent2", DateTimeOffset.Now, secondPublicationId, null, author),
                        }, 2)
                }
            };

            var model = new FeaturedQuery()
            {
                Keys = new string[] { firstPublicationId, secondPublicationId }
            };

            var serviceMock = new Mock<ICommentsService>();
            serviceMock
                .Setup(s => s.SearchFeaturedAsync(new string[] { firstPublicationId, secondPublicationId }))
                .ReturnsAsync(featuredComments);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.PostAsync("Comments/comments/featured", model.AsJsonContent());
            var result = await response.Content.DeserializeContent<Dictionary<string, FeaturedInfo>>();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);

            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result[firstPublicationId].Comments.Count() == 2);
            Assert.IsTrue(result[secondPublicationId].Comments.Count() == 2);
        }
    }
}