using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Publications.Api.UnitTest.PublicationsController
{
    [TestFixture]
    public class GetByIdTests
    {
        [Test]
        public async Task GetById_NotFound()
        {
            // Setup
            var id = "some-id";
            var serviceMock = new Mock<IPublicationService>();
            serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(default(Publication));
            
            var controller = new Controllers.PublicationsController(serviceMock.Object);

            // Act
            var response = await controller.GetByIdAsync(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(response.Result);
            var result = (StatusCodeResult)response.Result;
            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.Null(response.Value);
        }

        [Test]
        public async Task GetById_Ok()
        {
            // Setup
            var id = "some-id";
            var serviceMock = new Mock<IPublicationService>();
            serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(new Publication(id, It.IsAny<string>(), new List<string>(), It.IsAny<UserInfo>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()));

            var controller = new Controllers.PublicationsController(serviceMock.Object);

            // Act
            var response = await controller.GetByIdAsync(id);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(response.Result);
            var result = (OkObjectResult)response.Result;
            Assert.IsInstanceOf<Publication>(result.Value);
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
            var model = (Publication) result.Value;
            Assert.NotNull(model);
        }
    }
}
