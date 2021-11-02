using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Content.Api.Models;
using GhostNetwork.Content.Comments;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Comments.Api
{
    [TestFixture]
    public class UpdateTests
    {
        [Test]
        public async Task Update_NoContent()
        {
            // Setup
            const string commentId = "someId";
            var model = new UpdateCommentModel
            {
                Content = "New Content"
            };

            DomainResult domainResult = DomainResult.Success();

            Comment comment = new Comment(commentId, model.Content, DateTimeOffset.Now, null, null, null);

            var serviceMock = new Mock<ICommentsService>();
            serviceMock.Setup(s => s.UpdateAsync(commentId, model.Content)).ReturnsAsync(domainResult);
            serviceMock.Setup(s => s.GetByIdAsync(commentId)).ReturnsAsync(comment);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.PutAsync($"comments/{commentId}", model.AsJsonContent());
            Comment result = await response.Content.DeserializeContent<Comment>();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
        }

        [Test]
        public async Task Update_BadRequest_EmptyContent()
        {
            // Setup
            const string commentId = "someId";
            var model = new UpdateCommentModel();

            DomainResult domainResult = DomainResult.Success();

            Comment comment = new Comment(commentId, model.Content, DateTimeOffset.Now, null, null, null);

            var serviceMock = new Mock<ICommentsService>();
            serviceMock.Setup(s => s.UpdateAsync(commentId, model.Content)).ReturnsAsync(domainResult);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.PutAsync($"comments/{commentId}", model.AsJsonContent());

            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Update_NotFound()
        {
            // Setup
            const string commentId = "someId";
            var model = new UpdateCommentModel
            {
                Content = "New Content"
            };

            DomainResult domainResult = DomainResult.Success();

            var serviceMock = new Mock<ICommentsService>();
            serviceMock.Setup(s => s.UpdateAsync(commentId, model.Content)).ReturnsAsync(domainResult);
            serviceMock.Setup(s => s.GetByIdAsync(commentId)).ReturnsAsync(default(Comment));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.PutAsync($"comments/{commentId}", model.AsJsonContent());

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Update_AllowTimeToEdit_Expired_BadRequest()
        {
            // Setup
            const string commentId = "some_id";
            const string publicationId = "some_id";
            const string content = "some content";

            var comment = new Comment(
                commentId,
                content,
                DateTimeOffset.UtcNow.AddDays(-1),
                publicationId,
                replyCommentId: null,
                new UserInfo(Guid.Empty, "Some Name", null));

            var input = new UpdateCommentModel
            {
                Content = "new content"
            };

            var serviceMock = new Mock<ICommentsService>();
            var storageMock = new Mock<ICommentsStorage>();

            serviceMock
                .Setup(s => s.GetByIdAsync(commentId))
                .ReturnsAsync(comment);

            serviceMock
                .Setup(s => s.UpdateAsync(commentId, input.Content))
                .ReturnsAsync(DomainResult.Error("API error!"));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(provider => serviceMock.Object);
                collection.AddScoped(provider => storageMock.Object);
            });

            // Act
            var response = await client.PutAsync($"/comments/{commentId}/", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
