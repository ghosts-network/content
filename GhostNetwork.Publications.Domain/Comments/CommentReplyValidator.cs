using System.Threading.Tasks;
using Domain;
using Domain.Validation;

namespace GhostNetwork.Publications.Comments
{
    public class CommentReplyValidator : IValidator<CommentContext>
    {
        private readonly ICommentsStorage commentsStorage;

        public CommentReplyValidator(ICommentsStorage commentsStorage)
        {
            this.commentsStorage = commentsStorage;
        }

        public async Task<DomainResult> ValidateAsync(CommentContext context)
        {
            if (string.IsNullOrEmpty(context.ReplyId))
            {
                return DomainResult.Success();
            }

            var parentComment = await commentsStorage.FindOneByIdAsync(context.ReplyId);

            return parentComment == null
                ? DomainResult.Error("Parent comment not found")
                : DomainResult.Success();
        }
    }
}