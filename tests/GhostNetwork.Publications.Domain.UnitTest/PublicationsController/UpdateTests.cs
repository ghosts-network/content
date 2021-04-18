using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Publications.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Publications.Domain.UnitTest.PublicationsController
{
    [TestFixture]
    public class UpdateTests
    {
        [Test]
        public async Task Update_NoContent()
        {
            // Setup
            var id = "some_id";
            var input = new UpdatePublicationModel
            {
                Content = "some content"
            };

            var publication = new Publication(id, input.Content, Enumerable.Empty<string>(), null, DateTimeOffset.Now, DateTimeOffset.Now);

            var serviceMock = new Mock<IPublicationService>();
            serviceMock
                .Setup(s => s.UpdateAsync(id, input.Content))
                .ReturnsAsync(DomainResult.Success());

            serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(publication);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(provider => serviceMock.Object);
            });

            // Act
            var response = await client.PutAsync($"/publications/{id}/", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Test]
        public async Task Update_NotFound()
        {
            // Setup
            var id = "some_id";
            var input = new UpdatePublicationModel
            {
                Content = "some content"
            };

            var serviceMock = new Mock<IPublicationService>();

            serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(default(Publication));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(provider => serviceMock.Object);
            });

            // Act
            var response = await client.PutAsync($"/publications/{id}/", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task Update_EmptyContent_BadRequest()
        {
            // Setup
            var id = "some_id";
            var input = new UpdatePublicationModel
            {
                Content = null
            };

            var serviceMock = new Mock<IPublicationService>();

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(provider => serviceMock.Object);
            });

            // Act
            var response = await client.PutAsync($"/publications/{id}/", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task Create_ServiceError_BadRequest()
        {
            // Setup
            var id = "some_id";
            var input = new CreatePublicationModel
            {
                Content = "some content"
            };
            
            var publication = new Publication(id, input.Content, Enumerable.Empty<string>(), null, DateTimeOffset.Now, DateTimeOffset.Now);

            var serviceMock = new Mock<IPublicationService>();
            serviceMock
                .Setup(s => s.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(DomainResult.Error("Some error"));

            serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(publication);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(provider => serviceMock.Object);
            });

            // Act
            var response = await client.PutAsync($"/publications/{id}/", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}