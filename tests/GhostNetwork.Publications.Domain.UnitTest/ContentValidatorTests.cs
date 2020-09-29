using System.Threading.Tasks;
using NUnit.Framework;

namespace GhostNetwork.Publications.Domain.UnitTest
{
    [TestFixture]
    public class ContentValidatorTests
    {
        [Test]
        public async Task ContentValidator_ShouldReturnFalse_If_ForbiddenWordsFound()
        {
            // Setup
            var validation = new ForbiddenWordsValidator();
            var content = "#test text with forbidden word duck";
            var context = new PublicationContext(content);

            // Act
            var result = await validation.ValidateAsync(context);

            // Assert
            Assert.IsFalse(result.Success);
        }

        [Test]
        public async Task ContentValidator_ShouldReturnTrue_If_ForbiddenWordsNotFound()
        {
            // Setup
            var validation = new ForbiddenWordsValidator();
            var content = "#test text without forbidden word";
            var context = new PublicationContext(content);

            // Act
            var result = await validation.ValidateAsync(context);

            // Assert
            Assert.IsTrue(result.Success);
        }
    }
}
