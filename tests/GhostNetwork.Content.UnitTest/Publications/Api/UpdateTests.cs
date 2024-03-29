using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UpdateTests
    {
        private static IEnumerable<TestCaseData> positiveCases = new[]
        {
            new TestCaseData(Enumerable.Empty<UpdateMediaModel>()),
            new TestCaseData(null),
        };

        [TestCaseSource(nameof(positiveCases))]
        public async Task Update_NoContent(IEnumerable<UpdateMediaModel> mediaCase)
        {
            // Arrange
            var id = "some_id";
            var input = new UpdatePublicationModel
            {
                Content = "some content",
                Media = mediaCase
            };

            var media = input.Media?.Select(x => (Media)x).ToList() ?? new List<Media>();
            
            var publication = 
                new Publication(
                    id,
                    input.Content,
                    Enumerable.Empty<string>(),
                    new UserInfo(Guid.NewGuid(), "Name", null),
                    DateTimeOffset.Now,
                    DateTimeOffset.Now,
                    media);

            var serviceMock = new Mock<IPublicationService>();
            serviceMock
                .Setup(s => s.UpdateAsync(id, input.Content, media))
                .ReturnsAsync(DomainResult.Success());

            serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(publication);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.PutAsync($"/publications/{id}/", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Test]
        public async Task Update_NotFound()
        {
            // Arrange
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
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.PutAsync($"/publications/{id}/", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task Update_EmptyContent_BadRequest()
        {
            // Arrange
            var id = "some_id";
            var input = new UpdatePublicationModel
            {
                Content = null
            };

            var serviceMock = new Mock<IPublicationService>();

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.PutAsync($"/publications/{id}/", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task Create_ServiceError_BadRequest()
        {
            // Arrange
            var id = "some_id";
            var input = new CreatePublicationModel
            {
                Content = "some content"
            };

            var publication = new Publication(id, input.Content, Enumerable.Empty<string>(), null, DateTimeOffset.Now, DateTimeOffset.Now, Enumerable.Empty<Media>());

            var serviceMock = new Mock<IPublicationService>();
            serviceMock
                .Setup(s => s.UpdateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<Media>>()))
                .ReturnsAsync(DomainResult.Error("Some error"));

            serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(publication);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.PutAsync($"/publications/{id}/", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}