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
    public class CreateTests
    {
        [Test]
        public async Task Create_Created()
        {
            // Setup
            var input = new CreatePublicationModel
            {
                Content = "some content"
            };

            var id = "some_id";
            var publication = new Publication(id, input.Content, Enumerable.Empty<string>(), null, DateTimeOffset.Now, DateTimeOffset.Now);

            var serviceMock = new Mock<IPublicationService>();
            serviceMock
                .Setup(s => s.CreateAsync(It.IsAny<string>(), It.IsAny<UserInfo>()))
                .ReturnsAsync((DomainResult.Success(), id));

            serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(publication);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(provider => serviceMock.Object);
            });

            // Act
            var response = await client.PostAsync("/publications/", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [Test]
        public async Task Create_EmptyContent_BadRequest()
        {
            // Setup
            var input = new CreatePublicationModel
            {
                Content = null
            };

            var serviceMock = new Mock<IPublicationService>();

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(provider => serviceMock.Object);
            });

            // Act
            var response = await client.PostAsync("/publications/", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task Create_ServiceError_BadRequest()
        {
            // Setup
            var input = new CreatePublicationModel
            {
                Content = "some content"
            };

            var serviceMock = new Mock<IPublicationService>();
            serviceMock
                .Setup(s => s.CreateAsync(It.IsAny<string>(), It.IsAny<UserInfo>()))
                .ReturnsAsync((DomainResult.Error("Some error"), default));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(provider => serviceMock.Object);
            });

            // Act
            var response = await client.PostAsync("/publications/", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}