using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Publications.Domain.UnitTest
{
    [TestFixture]
    public class UpdatingPublicationTests
    {

        [Test]
        public async Task Test_Update_Publication()
        {
            // Setup
            var mock = new Mock<IPublicationStorage>();
            var list = new List<Publication>
            {
                new Publication("1", "text1", DateTimeOffset.Now, new List<string>(), DateTimeOffset.Now, false),
                new Publication("2", "text2", DateTimeOffset.Now, new List<string>(), DateTimeOffset.Now, false),
                new Publication("3", "text3", DateTimeOffset.Now, new List<string>(), DateTimeOffset.Now, false)
            };
            mock.Setup(repo => repo.FindOneByIdAsync(It.IsAny<string>())).
                ReturnsAsync((string id) => list.FirstOrDefault(x => x.Id == id));
            mock.Setup(repo => repo.UpdateOneAsync(It.IsAny<string>(), It.IsAny<Publication>())).ReturnsAsync(
                (string id, Publication publ) =>
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].Id == id)
                        {
                            var publication = new Publication(id, publ.Content, publ.CreatedOn, publ.Tags, publ.UpdatedOn, publ.IsUpdated);
                            list[i] = publication;
                        }
                    }

                    return true;
                });
            IPublicationStorage publicationStorage = mock.Object;
            IPublicationService service = new PublicationService(
                                                                new DefaultLengthValidator(),
                                                                new PublicationBuilder(new DefaultHashTagsFetcher()),
                                                                publicationStorage);

            // Act
            await service.UpdateOneAsync("1", "another text");
            var updatedPublication = await service.FindOneByIdAsync("1");

            // Assert
            Assert.IsTrue(updatedPublication.IsUpdated);
            Assert.AreEqual(updatedPublication.Content, "another text");
            Assert.AreNotEqual(updatedPublication.UpdatedOn, updatedPublication.CreatedOn);
        }
    }
}
