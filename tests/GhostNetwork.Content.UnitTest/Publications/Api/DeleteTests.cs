using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Content.Publications;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Publications.Api
{
    [TestFixture]
    public class DeleteTests
    {
        [Test]
        public async Task Delete_NoContent()
        {
            // Assert
            var id = "some_id";

            var publication = new Publication(id, It.IsAny<string>(), Enumerable.Empty<string>(), null, DateTimeOffset.Now, DateTimeOffset.Now);

            var serviceMock = new Mock<IPublicationService>();

            serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(publication);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.DeleteAsync($"/publications/{id}/");

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Test]
        public async Task Delete_NotFound()
        {
            // Assert
            var id = "some_id";

            var serviceMock = new Mock<IPublicationService>();

            serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(default(Publication));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.DeleteAsync($"/publications/{id}/");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}