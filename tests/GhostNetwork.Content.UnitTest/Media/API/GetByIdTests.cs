using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Content.MediaContent;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Media.API;

[TestFixture]
public class GetByIdTests
{
	[Test]
	public async Task GetById_Ok()
	{
		//Arrange
		var id = "some_id";

		var media = new MediaContent.Media(id, It.IsAny<string>(), It.IsAny<string>());

		var serviceMock = new Mock<IMediaService>();

		serviceMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(media);
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddScoped(_ => serviceMock.Object);
		});
		
		//Act
		var response = await client.GetAsync($"/media/{id}");
		
		//Assert
		Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
	}

	[Test]
	public async Task GetById_NotFound()
	{
		//Arrange
		var id = "some_id";

		var serviceMock = new Mock<IMediaService>();

		serviceMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(default(MediaContent.Media));
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddScoped(_ => serviceMock.Object);
		});
		
		//Act
		var response = await client.GetAsync($"/media/{id}");
		
		//Assert
		Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
	}
}