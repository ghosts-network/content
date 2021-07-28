﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Domain;
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

            const string content = "New Content";

            DomainResult domainResult = DomainResult.Success();

            Comment comment = new Comment(commentId, content, DateTimeOffset.Now, null, null, null);

            var serviceMock = new Mock<ICommentsService>();
            serviceMock.Setup(s => s.UpdateAsync(commentId, content)).ReturnsAsync(domainResult);
            serviceMock.Setup(s => s.GetByIdAsync(commentId)).ReturnsAsync(comment);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.PutAsync($"comments/{commentId}", content.AsJsonContent());
            Comment result = await response.Content.DeserializeContent<Comment>();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
        }

      [Test]
      public async Task Update_BadRequest_EmptyContent()
        {
            // Setup
            const string commentId = "someId";

            const string content = "";

            DomainResult domainResult = DomainResult.Success();

            Comment comment = new Comment(commentId, content, DateTimeOffset.Now, null, null, null);

            var serviceMock = new Mock<ICommentsService>();
            serviceMock.Setup(s => s.UpdateAsync(commentId, content)).ReturnsAsync(domainResult);

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

            DomainResult domainResult = DomainResult.Success();

            var serviceMock = new Mock<ICommentsService>();
            serviceMock.Setup(s => s.UpdateAsync(commentId, content)).ReturnsAsync(domainResult);
            serviceMock.Setup(s => s.GetByIdAsync(commentId)).ReturnsAsync(default(Comment));

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
