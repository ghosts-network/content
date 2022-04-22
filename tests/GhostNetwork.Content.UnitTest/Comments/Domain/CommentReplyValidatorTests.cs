using System;
using System.Linq;
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
            var comment = new Comment(string.Empty, string.Empty, DateTimeOffset.Now, string.Empty, replyCommentId: null, null);

            // Act
            var result = await validator.ValidateAsync(comment);

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
            var comment = new Comment(string.Empty, string.Empty, DateTimeOffset.Now, string.Empty, replyCommentId: "replyId", null);

            // Act
            var result = await validator.ValidateAsync(comment);

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
            var comment = new Comment(string.Empty, string.Empty, DateTimeOffset.Now, string.Empty, replyCommentId: "replyId", null);

            // Act
            var result = await validator.ValidateAsync(comment);

            // Assert
            Assert.IsTrue(result.Successed);
        }
    }
}