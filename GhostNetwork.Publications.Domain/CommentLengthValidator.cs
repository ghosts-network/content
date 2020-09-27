using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain
{
    public class CommentLengthValidator : ICommentLengthValidator
    {
        private readonly int? maxLength;

        public CommentLengthValidator(int? maxLength = null)
        {
            this.maxLength = maxLength;
        }

        public Task<DomainResult> ValidateAsync(CommentContext content)
        {
            if (maxLength == null)
            {
                return Task.FromResult(DomainResult.Successed());
            }

            if (content.Content.Length > maxLength)
            {
                return Task.FromResult(DomainResult.Error($"Content is more than {maxLength.Value} characters"));
            }

            return Task.FromResult(DomainResult.Successed());
        }
    }
}
