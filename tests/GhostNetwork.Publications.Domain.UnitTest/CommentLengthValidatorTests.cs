using System.Threading.Tasks;
using NUnit.Framework;

namespace GhostNetwork.Publications.Domain.UnitTest
{
    [TestFixture]
    public class CommentLengthValidatorTests
    {
        [Test]
        public async Task Length_Validator_Without_Param()
        {
            // Setup
            var validator = new CommentLengthValidator();
            var context = new CommentContext("simple text");

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsTrue(result.Success);
        }

        [Test]
        public async Task Length_Validator_Max_Characters_5()
        {
            // Setup
            var validator = new CommentLengthValidator(5);
            var context = new CommentContext("simple text");
            var context2 = new CommentContext("12345");

            // Act
            var result = await validator.ValidateAsync(context);
            var result2 = await validator.ValidateAsync(context2);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result2.Success);
        }
    }
}
