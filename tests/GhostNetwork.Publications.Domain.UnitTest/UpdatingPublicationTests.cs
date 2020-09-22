using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Publications.Domain.UnitTest
{
    [TestFixture]
    public class UpdatingPublicationTests
    {
        [Test]
        public async Task Test_Storage()
        {
            // Setup
            var mock = new Mock<IPublicationStorage>();
            mock.Setup(repo => repo.UpdateOneAsync(It.IsAny<string>(), It.IsAny<Publication>())).ReturnsAsync(
                (string id, Publication publication) => true);

            IPublicationStorage publicationStorage = mock.Object;
            IPublicationService service = new PublicationService(
                new DefaultLengthValidator(),
                new PublicationBuilder(new DefaultHashTagsFetcher()),
                publicationStorage);

            // Act
            var result = await service.UpdateOneAsync("1", "another text");

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_Update_Publication()
        {
            // Setup
            var publication = new Publication("1", "text1", DateTimeOffset.MinValue, new List<string>(), DateTimeOffset.Now);

            // Assert
            Assert.IsTrue(publication.IsUpdated);
        }

        [Test]
        public void Test_Update_Publication2()
        {
            // Setup
            var publication = new Publication("1", "text1", DateTimeOffset.Now, new List<string>(), DateTimeOffset.Now);

            // Assert
            Assert.IsFalse(publication.IsUpdated);
        }
    }
}