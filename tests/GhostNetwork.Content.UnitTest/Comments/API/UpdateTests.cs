using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GhostNetwork.Content.Api.Helpers;
using GhostNetwork.Content.Api.Models;
using GhostNetwork.Content.Comments;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Domain;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Comments.Api
{
    [TestFixture]
    public class UpdateTests
    {
      [Test]
      public async Task Update_Ok()
        {
            // Setup
            const string commentId = "someId";

            const string content = "New Content";

            DomainResult domainResult = DomainResult.Success();

            Comment comment = new Comment(commentId, content, DateTimeOffset.Now, null, null, null);

            var serviceMock = new Mock<ICommentsService>();
            serviceMock.Setup(s => s.UpdateAsync(commentId, content)).ReturnsAsync((domainResult, comment));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.PutAsync($"comments/{commentId}", content.AsJsonContent());
            Comment result = await response.Content.DeserializeContent<Comment>();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(result, comment);
        }

      [Test]
      public async Task Update_NoContent()
        {
            // Setup
            const string commentId = "someId";

            const string content = "New Content";

            DomainResult domainResult = DomainResult.Success();

            var serviceMock = new Mock<ICommentsService>();
            serviceMock.Setup(s => s.UpdateAsync(commentId, content)).ReturnsAsync((domainResult, null));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.PutAsync($"comments/{commentId}", content.AsJsonContent());

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
        }

      [Test]
      public async Task Update_BadRequest()
        {
            // Setup
            const string commentId = "someId";

            const string content = "New Content";

            DomainResult domainResult = DomainResult.Error(new DomainError("Some Error"));

            var serviceMock = new Mock<ICommentsService>();
            serviceMock.Setup(s => s.UpdateAsync(commentId, content)).ReturnsAsync((domainResult, null));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.PutAsync($"comments/{commentId}", content.AsJsonContent());

            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

      [Test]
      public async Task Update_NotFound()
        {
            // Setup
            const string commentId = "someId";

            const string content = "New Content";

            DomainResult domainResult = DomainResult.Error(new DomainError("Not Found"));

            var serviceMock = new Mock<ICommentsService>();
            serviceMock.Setup(s => s.UpdateAsync(commentId, content)).ReturnsAsync((domainResult, null));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.PutAsync($"comments/{commentId}", content.AsJsonContent());

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }
    }
}
