using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GhostNetwork.Publications.Domain.UnitTest
{
    [TestFixture]
    public class DefaultLengthValidatorTests
    {
        [Test]
        public async Task Length_Without_Params()
        {
            // Setup
            var validator = new LengthValidator();
            var content = "Hello, world";
            var context = new PublicationContext(content);

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsTrue(result.Success);
        }

        [Test]
        public async Task Length_Without_Params_Empty_String()
        {
            // Setup
            var validator = new LengthValidator();
            var content = string.Empty;
            var context = new PublicationContext(content);

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsTrue(result.Success);
        }

        [Test]
        public async Task Length_Must_Be_Less_Or_Equal_Than_3_Chars()
        {
            // Setup
            var validator = new LengthValidator(3);
            var content = "Hello";
            var context = new PublicationContext(content);

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsFalse(result.Success);
        }

        [Test]
        public async Task Length_Must_Be_Less_Or_Equal_Than_5_Chars()
        {
            // Setup
            var validator = new LengthValidator(5);
            var content = "Test";
            var context = new PublicationContext(content);

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsTrue(result.Success);
        }

        [Test]
        public async Task Length_Equally_String_Length()
        {
            // Setup
            var validator = new LengthValidator(5);
            var content = "12345";
            var context = new PublicationContext(content);

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsTrue(result.Success);
        }
    }
}
