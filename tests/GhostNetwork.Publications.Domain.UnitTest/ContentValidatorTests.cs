using NUnit.Framework;

namespace GhostNetwork.Publications.Domain.UnitTest
{
    [TestFixture]
    class ContentValidatorTests
    {
        [Test]
        public void ContentValidator_ShouldReturnFalse_If_ForbiddenWordsFound()
        {
            // Setup
            var validation = new ContentValidator();
            var content = "#test text with forbidden word duck";

            // Act
            var result = validation.FindeForbiddenWords(content);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ContentValidator_ShouldReturnTrue_If_ForbiddenWordsNotFound()
        {
            // Setup
            var validation = new ContentValidator();
            var content = "#test text without forbidden word";

            // Act
            var result = validation.FindeForbiddenWords(content);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
