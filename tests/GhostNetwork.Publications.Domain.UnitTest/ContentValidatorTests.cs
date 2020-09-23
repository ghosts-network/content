using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace GhostNetwork.Publications.Domain.UnitTest
{
    [TestFixture]
    public class ContentValidatorTests
    {
        [Test]
        public void MustReturnFalse_if_TextContain_ForbiddenWord()
        {
            // Setup
            var validator = new ContentValidator();
            string content = "Simple text with forbidden duck";

            // Act
            var result = validator.FindenWords(content);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void MustReturnTrue_if_TextDoesntContain_ForbiddenWord()
        {
            // Setup
            var validator = new ContentValidator();
            string content = "Simple text with forbidden";

            // Act
            var result = validator.FindenWords(content);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
