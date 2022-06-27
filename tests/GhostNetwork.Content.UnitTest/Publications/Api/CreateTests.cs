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
    public class CreateTests
    {
        [Test]
        public async Task Create_Created()
        {
            // Assert
            var input = new CreatePublicationModel
            {
                Content = "some content",
                Media = Array.Empty<CreateMediaModel>()
            };

            var id = "some_id";
            var media = input.Media.Select(x => new Media(default, x.Link));
            var publication = new Publication(id, input.Content, Enumerable.Empty<string>(), null, DateTimeOffset.Now, DateTimeOffset.Now, media);

            var serviceMock = new Mock<IPublicationService>();
            serviceMock
                .Setup(s => s.CreateAsync(It.IsAny<string>(), It.IsAny<UserInfo>(), publication.Media))
                .ReturnsAsync((DomainResult.Success(), id));

            serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(publication);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.PostAsync("/publications/", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

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
                Media = new List<CreateMediaModel>
                {
                    new()
                    {
                        Link = "Link"
                    }
                }
            };

            var serviceMock = new Mock<IPublicationService>();
            serviceMock
                .Setup(s => s.CreateAsync(It.IsAny<string>(), It.IsAny<UserInfo>(), It.IsAny<IEnumerable<Media>>()))
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