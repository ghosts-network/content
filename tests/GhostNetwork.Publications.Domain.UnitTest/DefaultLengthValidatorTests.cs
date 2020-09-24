using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace GhostNetwork.Publications.Domain.UnitTest
{
    [TestFixture]
    public class DefaultLengthValidatorTests
    {
        [Test]
        public void Length_Without_Params()
        {
            // Setup
            var validator = new LengthValidator();
            string text = "Hello, world";

            // Act
            var result = validator.Validate(text);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Length_Without_Params_Empty_String()
        {
            // Setup
            var validator = new LengthValidator();
            string text = string.Empty;

            // Act
            var result = validator.Validate(text);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Length_Must_Be_Less_Or_Equal_Than_3_Chars()
        {
            // Setup
            var validator = new LengthValidator(3);
            string text = "Hello";

            // Act
            var result = validator.Validate(text);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Length_Must_Be_Less_Or_Equal_Than_5_Chars()
        {
            // Setup
            var validator = new LengthValidator(5);
            string text = "Test";

            // Act
            var result = validator.Validate(text);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Length_Equally_String_Length()
        {
            // Setup
            var validator = new LengthValidator(5);
            string text = "12345";

            // Act
            var result = validator.Validate(text);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
