using System;
using System.Collections.Generic;
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
            var mock2 = new Mock<ICommentsStorage>();
            mock
                .Setup(repo => repo.UpdateOneAsync(It.IsAny<string>(), It.IsAny<Publication>()))
                .ReturnsAsync(true);
            var validators = new PublicationValidatorsContainer(
                new LengthValidator(),
                new ForbiddenWordsValidator(new[]
                {
                    new ForbiddenWordModel
                    {
                        ForbiddenWord = "duck"
                    }
                }));

            var service = new PublicationService(
                validators,
                new PublicationBuilder(new DefaultHashTagsFetcher()),
                mock.Object, mock2.Object);

            // Act
            var result = await service.UpdateOneAsync("1", "another text");

            // Assert
            Assert.IsTrue(result.Success);
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
            var time = DateTimeOffset.Now;
            var publication = new Publication("1", "text1", time, new List<string>(), time);

            // Assert
            Assert.IsFalse(publication.IsUpdated);
        }
    }
}