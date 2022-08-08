using System.Net;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Content.Api.Models;
using GhostNetwork.Content.MediaContent;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Media.API;

[TestFixture]
public class CreateTests
{
	[Test]
	public async Task CreateMedia_Created()
	{
		// Arrange
		var model = new CreateMediaModel()
		{
			Key = "key",
			Link = "https://some.com/some.png"
		};

		var media = new MediaContent.Media("someId", model.Link, model.Key);

		var serviceMock = new Mock<IMediaService>();

		serviceMock
			.Setup(x => x.InsertAsync(model.Link, model.Key))
			.ReturnsAsync((DomainResult.Success(), media.Id));

		serviceMock.Setup(x => x.GetByIdAsync(media.Id)).ReturnsAsync(media);
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddScoped(_ => serviceMock.Object);
		});
		
		// Act
		var response = await client.PostAsync("/media", model.AsJsonContent());
		
		// Assert
		Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
	}
	
	[Test]
	public async Task CreateMedia_BadRequest()
	{
		// Arrange
		var model = new CreateMediaModel()
		{
			Key = "key",
			Link = "https://some.com/some.png"
		};

		var serviceMock = new Mock<IMediaService>();

		serviceMock
			.Setup(x => x.InsertAsync(model.Link, model.Key))
			.ReturnsAsync((DomainResult.Error("Some error"), default));

		var client = TestServerHelper.New(collection =>
		{
			collection.AddScoped(_ => serviceMock.Object);
		});
		
		// Act
		var response = await client.PostAsync("/media", model.AsJsonContent());
		
		// Assert
		Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
	}
}