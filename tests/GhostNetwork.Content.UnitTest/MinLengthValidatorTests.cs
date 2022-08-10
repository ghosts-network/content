using System;
using System.Linq;
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

            var publication = new Publication(
                string.Empty,
                content,
                Enumerable.Empty<string>(),
                null,
                DateTimeOffset.Now,
                DateTimeOffset.Now,
                Enumerable.Empty<Media>());

            // Act
            var result = await validator.ValidateAsync(publication);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task Publication_Content_Length_Shorter_Than_MinLength()
        {
            // Setup
            var validator = new MinLengthValidator(5);
            var content = "Hi";

            var publication = new Publication(
                string.Empty,
                content,
                Enumerable.Empty<string>(),
                null,
                DateTimeOffset.Now,
                DateTimeOffset.Now,
                Enumerable.Empty<Media>());

            // Act
            var result = await validator.ValidateAsync(publication);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task Publication_Content_Length_Equal_MinLength()
        {
            // Setup
            var validator = new MinLengthValidator(5);
            var content = "Hello";

            var publication = new Publication(
                string.Empty,
                content,
                Enumerable.Empty<string>(),
                null,
                DateTimeOffset.Now,
                DateTimeOffset.Now,
                Enumerable.Empty<Media>());

            // Act
            var result = await validator.ValidateAsync(publication);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task Comment_Content_Length_Longer_Than_MinLength()
        {
            // Setup
            var validator = new MinLengthValidator(5);
            var content = "Hello_Hello";

            var comment = new Comment(string.Empty, content, DateTimeOffset.Now, string.Empty, replyCommentId: null, null);

            // Act
            var result = await validator.ValidateAsync(comment);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task Comment_Content_Length_Shorter_Than_MinLength()
        {
            // Setup
            var validator = new MinLengthValidator(5);
            var content = "Hi";

            var comment = new Comment(string.Empty, content, DateTimeOffset.Now, string.Empty, replyCommentId: null, null);

            // Act
            var result = await validator.ValidateAsync(comment);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task Comment_Content_Length_Equal_MinLength()
        {
            // Setup
            var validator = new MinLengthValidator(5);
            var content = "Hello";

            var comment = new Comment(string.Empty, content, DateTimeOffset.Now, string.Empty, replyCommentId: null, null);

            // Act
            var result = await validator.ValidateAsync(comment);

            // Assert
            Assert.IsTrue(result.Successed);
        }
    }
}
