using System.Threading.Tasks;
using Domain;
using Domain.Validation;

namespace GhostNetwork.Content.Comments
{
    public class CommentReplyValidator : IValidator<Comment>
    {
        private readonly ICommentsStorage commentsStorage;

        public CommentReplyValidator(ICommentsStorage commentsStorage)
        {
            this.commentsStorage = commentsStorage;
        }

        public async Task<DomainResult> ValidateAsync(Comment comment)
        {
            if (string.IsNullOrEmpty(comment.ReplyCommentId))
            {
                return DomainResult.Success();
            }

            var parentComment = await commentsStorage.FindOneByIdAsync(comment.ReplyCommentId);

            return parentComment == null
                ? DomainResult.Error("Parent comment not found")
                : DomainResult.Success();
        }
    }
}