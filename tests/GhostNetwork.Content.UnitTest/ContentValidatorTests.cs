using System;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Content.Publications;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest
{
    [TestFixture]
    public class ContentValidatorTests
    {
        [Test]
        public async Task ContentValidator_ShouldReturnFalse_If_ForbiddenWordsFound()
        {
            // Setup
            var validation = new ForbiddenWordsValidator(new[]
            {
                "duck"
            });

            var content = "#test text with forbidden word duck";
            var publication = new Publication(
                string.Empty,
                content,
                Enumerable.Empty<string>(),
                null,
                DateTimeOffset.Now,
                DateTimeOffset.Now,
                Enumerable.Empty<Media>());

            // Act
            var result = await validation.ValidateAsync(publication);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task ContentValidator_ShouldReturnTrue_If_ForbiddenWordsNotFound()
        {
            // Setup
            var validation = new ForbiddenWordsValidator(new[]
            {
                "duck"
            });

            var content = "#test text without forbidden word";
            var publication = new Publication(
                string.Empty,
                content,
                Enumerable.Empty<string>(),
                null,
                DateTimeOffset.Now,
                DateTimeOffset.Now,
                Enumerable.Empty<Media>());

            // Act
            var result = await validation.ValidateAsync(publication);

            // Assert
            Assert.IsTrue(result.Successed);
        }
    }
}
