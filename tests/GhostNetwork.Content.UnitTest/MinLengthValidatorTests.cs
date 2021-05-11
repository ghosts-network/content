using System;
using System.Threading.Tasks;
using GhostNetwork.Content.Comments;
using GhostNetwork.Content.Publications;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest
{
    [TestFixture]
    public class MinLengthValidatorTests
    {
        [Test]
        public void Invalid_MinLength_Parameter()
        {
            // Assert
            Assert.Throws<ArgumentException>(() => new MinLengthValidator(-1));
        }

        [Test]
        public async Task Publication_Content_Length_Longer_Than_MinLength()
        {
            // Setup
            var validator = new MinLengthValidator(5);
            var content = "Hello_Hello";
            var context = new PublicationContext(content);

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task Publication_Content_Length_Shorter_Than_MinLength()
        {
            // Setup
            var validator = new MinLengthValidator(5);
            var content = "Hi";
            var context = new PublicationContext(content);

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task Publication_Content_Length_Equal_MinLength()
        {
            // Setup
            var validator = new MinLengthValidator(5);
            var content = "Hello";
            var context = new PublicationContext(content);

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task Comment_Content_Length_Longer_Than_MinLength()
        {
            // Setup
            var validator = new MinLengthValidator(5);
            var content = "Hello_Hello";
            var context = new CommentContext(content);

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task Comment_Content_Length_Shorter_Than_MinLength()
        {
            // Setup
            var validator = new MinLengthValidator(5);
            var content = "Hi";
            var context = new CommentContext(content);

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task Comment_Content_Length_Equal_MinLength()
        {
            // Setup
            var validator = new MinLengthValidator(5);
            var content = "Hello";
            var context = new CommentContext(content);

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsTrue(result.Successed);
        }
    }
}
