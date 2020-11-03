using System.Threading.Tasks;
using NUnit.Framework;

namespace GhostNetwork.Publications.Domain.UnitTest
{
    [TestFixture]
    public class MaxLengthValidatorTests
    {
        [Test]
        public async Task Length_Must_Be_Less_Or_Equal_Than_3_Chars()
        {
            // Setup
            var validator = new MaxLengthValidator(3);
            var content = "Hello";
            var context = new PublicationContext(content);

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task Length_Must_Be_Less_Or_Equal_Than_5_Chars()
        {
            // Setup
            var validator = new MaxLengthValidator(5);
            var content = "Test";
            var context = new PublicationContext(content);

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task Length_Equally_String_Length()
        {
            // Setup
            var validator = new MaxLengthValidator(5);
            var content = "12345";
            var context = new PublicationContext(content);

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsTrue(result.Successed);
        }
    }
}
