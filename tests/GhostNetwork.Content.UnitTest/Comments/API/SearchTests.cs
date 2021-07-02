using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using GhostNetwork.Content.Comments;
using Moq;
using NUnit.Framework;

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
            
            int commentId = 1;

            var author = new UserInfo(Guid.NewGuid(), "SomeName", null);

            var comments = new Comment[]
            {
                new Comment((commentId++).ToString(), "someContent1", DateTimeOffset.Now, publicationId, null, author),
                new Comment((commentId++).ToString(), "someContent2", DateTimeOffset.Now, publicationId, null, author),
                new Comment((commentId++).ToString(), "someContent3", DateTimeOffset.Now, publicationId, null, author)
            };

            var serviceMock = new Mock<ICommentsService>();
            serviceMock.Setup(s => s.SearchAsync(publicationId, skip, take)).ReturnsAsync((comments, comments.Count()));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.GetAsync($"comments/bypublication/{publicationId}?skip={skip}&take={take}");
            var result = await response.Content.DeserializeContent<IEnumerable<Comment>>();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(result.Count(), 3);
        }
    }
}