using Domain;
using GhostNetwork.Content.Api.Models;
using GhostNetwork.Content.Comments;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GhostNetwork.Content.UnitTest.Comments.Api
{
    [TestFixture]
    public class CreateTests
    {
        [Test]
        public async Task Create_Ok()
        {
            // Setup
            const string commentId = "commentId";
            const string publicationId = "publicationId";
            const string content = "asd";
            const string authorId = "3fa85f64-5717-4562-b3fc-2c963f66afa7";

            var author = new UserInfo(new Guid(authorId), "FName LName", null);

            var model = new CreateCommentModel()
            {
                Key = publicationId,
                Content = content,
                ReplyCommentId = null,
                AuthorId = authorId
            };

            var commentServiceMock = new Mock<ICommentsService>();
            commentServiceMock
                .Setup(s => s.CreateAsync(publicationId, content, null, author))
                .ReturnsAsync((DomainResult.Success(), "commentId"));

            commentServiceMock
                .Setup(s => s.GetByIdAsync(commentId))
                .ReturnsAsync(new Comment(commentId, content, DateTimeOffset.Now, publicationId, null, author));

            var userProvideMock = new Mock<IUserProvider>();
            userProvideMock
                .Setup(s => s.GetByIdAsync(model.AuthorId))
                .ReturnsAsync(author);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => commentServiceMock.Object);
                collection.AddScoped(_ => userProvideMock.Object);
            });

            // Act
            var response = await client.PostAsync("comments", model.AsJsonContent());

            // Asser
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
        }

        [Test]
        public async Task Create_BadRequest()
        {
            // Setup
            const string publicationId = "publicationId";
            const string content = "asd";
            const string authorId = "3fa85f64-5717-4562-b3fc-2c963f66afa7";

            var author = new UserInfo(new Guid(authorId), "FName LName", null);

            var model = new CreateCommentModel()
            {
                Key = publicationId,
                Content = content,
                ReplyCommentId = null,
                AuthorId = authorId
            };

            var serviceMock = new Mock<ICommentsService>();
            serviceMock
                .Setup(s => s.CreateAsync(publicationId, content, null, author))
                .ReturnsAsync((DomainResult.Error("somethig went wrong"), null));

            var userProvideMock = new Mock<IUserProvider>();
            userProvideMock
                .Setup(s => s.GetByIdAsync(model.AuthorId))
                .ReturnsAsync(author);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
                collection.AddScoped(_ => userProvideMock.Object);
            });

            // Act
            var response = await client.PostAsync("comments", model.AsJsonContent());

            // Asser
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }
    }
}