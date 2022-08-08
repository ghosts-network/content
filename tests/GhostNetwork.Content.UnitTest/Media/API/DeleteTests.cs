using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Content.MediaContent;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Media.API;

[TestFixture]
public class DeleteTests
{
	[Test]
	public async Task DeleteById_NoContent()
	{
		// Arrange
		var id = "someId";

		var media = new MediaContent.Media(id, "link", "key");

		var serviceMock = new Mock<IMediaService>();

		serviceMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(media);
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddScoped(_ => serviceMock.Object);
		});
		
		// Act
		var response = await client.DeleteAsync($"/media/{id}");
		
		// Assert
		Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
	}
	
	[Test]
	public async Task DeleteById_NotFound()
	{
		// Arrange
		var id = "someId";

		var serviceMock = new Mock<IMediaService>();

		serviceMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(default(MediaContent.Media));
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddScoped(_ => serviceMock.Object);
		});
		
		// Act
		var response = await client.DeleteAsync($"/media/{id}");
		
		// Assert
		Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
	}
}