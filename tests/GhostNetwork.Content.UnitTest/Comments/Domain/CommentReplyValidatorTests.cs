using System;
using System.Threading.Tasks;
using GhostNetwork.Content.Comments;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Comments.Domain
{
    [TestFixture]
    public class CommentReplyValidatorTests
    {
        [Test]
        public async Task ReplyId_Is_Empty()
        {
            // Setup
            var commentsStorageMock = new Mock<ICommentsStorage>();

            var validator = new CommentReplyValidator(commentsStorageMock.Object);
            var context = new CommentContext("Hello_Hello");

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task ReplyId_Does_Not_Exists()
        {
            // Setup
            var commentsStorageMock = new Mock<ICommentsStorage>();
            commentsStorageMock
                .Setup(s => s.FindOneByIdAsync("replyId"))
                .ReturnsAsync(default(Comment));

            var validator = new CommentReplyValidator(commentsStorageMock.Object);
            var context = new CommentContext("Hello_Hello", "replyId");

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task ReplyId_Exists()
        {
            // Setup
            var commentsStorageMock = new Mock<ICommentsStorage>();
            commentsStorageMock
                .Setup(s => s.FindOneByIdAsync("replyId"))
                .ReturnsAsync(new Comment(default, default, DateTimeOffset.Now, default, default, default));

            var validator = new CommentReplyValidator(commentsStorageMock.Object);
            var context = new CommentContext("Hello_Hello", "replyId");

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            Assert.IsTrue(result.Successed);
        }
    }
}