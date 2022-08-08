using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Content.Api.Models;
using GhostNetwork.Content.MediaContent;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Media.API;

[TestFixture]
public class SearchTests
{
	[Test]
	public async Task SearchByKey_Ok()
	{
		//Arrange
		var key = "someKey";

		var media = new List<MediaContent.Media>() {new ("id", "https://klike.net/image.jpg", key)};

		var serviceMock = new Mock<IMediaService>();
		
		serviceMock.Setup(x => x.SearchByKeyAsync(key)).ReturnsAsync(media);
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddScoped(_ => serviceMock.Object);
		});
		
		//Act
		var response = await client.GetAsync($"media/bykey/{key}");

		//Assert
		Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
	}

	[Test]
	public async Task FindGroupedMedia_Ok()
	{
		//Arrange
		const string firstKey = "firstKey";
		const string secondKey = "secondKey";
		
		var model = new MediaQuery()
		{
			Keys = new string[] { firstKey, secondKey }
		};

		var groupedMedia = new Dictionary<string, GroupedMedia>
		{
			{   firstKey, new GroupedMedia(
					new []{new MediaContent.Media("id1", "https://klike.net/image.jpg", "key1")}, 1)
			},
			{
				secondKey, new GroupedMedia(
					new []
					{
						new MediaContent.Media("id2", "https://klike.net/image.jpg", "key2"),
						new MediaContent.Media("id3", "https://klike.net/image.jpg", "key2")
					}, 2)
			}
		};
		
		var serviceMock = new Mock<IMediaService>();
		
		serviceMock.Setup(x => x.FindGroupedMediaAsync(model.Keys)).ReturnsAsync(groupedMedia);
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddScoped(_ => serviceMock.Object);
		});
		
		//Act
		var response = await client.PostAsync("media/grouped", model.AsJsonContent());
		var result = await response.Content.DeserializeContent<Dictionary<string, GroupedMedia>>();
		
		// Assert
		Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);

		Assert.IsTrue(result.Count == 2);
		Assert.IsTrue(result[firstKey].Media.Count() == 1);
		Assert.IsTrue(result[secondKey].Media.Count() == 2);
	}
}