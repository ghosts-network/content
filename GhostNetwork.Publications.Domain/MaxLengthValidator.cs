using System.Threading.Tasks;
using Domain;
using Domain.Validation;
using GhostNetwork.Publications.Comments;

namespace GhostNetwork.Publications
{
    public class MaxLengthValidator : IValidator<PublicationContext>,
        IValidator<CommentContext>
    {
        private readonly int maxLength;

        public MaxLengthValidator(int maxLength)
        {
            this.maxLength = maxLength;
        }

        public Task<DomainResult> ValidateAsync(PublicationContext context)
        {
            return Task.FromResult(Validate(context.Content));
        }

        public Task<DomainResult> ValidateAsync(CommentContext context)
        {
            return Task.FromResult(Validate(context.Content));
        }

        private DomainResult Validate(string content)
        {
            return content.Length > maxLength
                ? DomainResult.Error($"Content length is more than {maxLength} characters")
                : DomainResult.Success();
        }
    }
}
