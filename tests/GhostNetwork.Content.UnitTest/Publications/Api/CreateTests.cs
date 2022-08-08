using System.Net;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Content.Api.Models;
using GhostNetwork.Content.Publications;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Publications.Api
{
    [TestFixture]
    public class CreateTests
    {
        [Test]
        public async Task Create_EmptyContent_BadRequest()
        {
            // Assert
            var input = new CreatePublicationModel
            {
                Content = null
            };

            var serviceMock = new Mock<IPublicationService>();

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.PostAsync("/publications/", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task Create_ServiceError_BadRequest()
        {
            // Assert
            var input = new CreatePublicationModel
            {
                Content = "some content",
            };

            var serviceMock = new Mock<IPublicationService>();
            serviceMock
                .Setup(s => s.CreateAsync(It.IsAny<string>(), It.IsAny<UserInfo>()))
                .ReturnsAsync((DomainResult.Error("Some error"), default));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.PostAsync("/publications/", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}