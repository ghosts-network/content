using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Content.Comments;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Comments.Api
{
    [TestFixture]
    public class GetByIdTests
    {
        [Test]
        public async Task GetById_Ok()
        {
            // Setup
            const string commentId = "someId";
            const string publicationId = "somePublicationId";
            const string author = "Some Author";

            var comment = new Comment(
                commentId,
                "content",
                DateTimeOffset.Now,
                publicationId,
                null,
                new UserInfo(Guid.Empty, author, null),
                Enumerable.Empty<Comment>());

            var serviceMock = new Mock<ICommentsService>();
            serviceMock.Setup(s => s.GetByIdAsync(commentId)).ReturnsAsync(comment);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.GetAsync($"/comments/{commentId}");

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetById_NotFound()
        {
            // Setup
            const string commentId = "someId";

            var serviceMock = new Mock<ICommentsService>();
            object p = serviceMock.Setup(s => s.GetByIdAsync(commentId)).ReturnsAsync(default(Comment));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.GetAsync($"/comments/{commentId}");

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }
    }
}