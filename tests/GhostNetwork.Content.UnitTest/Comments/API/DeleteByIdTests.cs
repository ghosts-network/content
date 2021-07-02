using System;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Content.Comments;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Comments.Api
{
    [TestFixture]
    public class DeleteByIdTests
    {
        [Test]
        public async Task DeleteById_NoContent()
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
                new UserInfo(Guid.Empty, author, null));

            var serviceMock = new Mock<ICommentsService>();
            serviceMock.Setup(s => s.GetByIdAsync(commentId)).ReturnsAsync(comment);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.DeleteAsync($"/comments/{commentId}");

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
        }

        [Test]
        public async Task DeleteById_NotFound()
        {
            // Setup
            const string commentId = "someId";

            var serviceMock = new Mock<ICommentsService>();
            serviceMock.Setup(s => s.GetByIdAsync(commentId)).ReturnsAsync(await Task.FromResult<Comment>(null));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.DeleteAsync($"/comments/{commentId}");

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }
    }
}