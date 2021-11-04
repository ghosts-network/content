using System.Threading.Tasks;
using Domain;
using Domain.Validation;
using GhostNetwork.Content.Comments;
using GhostNetwork.Content.Publications;

namespace GhostNetwork.Content
{
    public class MaxLengthValidator : IValidator<Publication>,
        IValidator<Comment>
    {
        private readonly int maxLength;

        public MaxLengthValidator(int maxLength)
        {
            this.maxLength = maxLength;
        }

        public Task<DomainResult> ValidateAsync(Publication publication)
        {
            return Task.FromResult(Validate(publication.Content));
        }

        public Task<DomainResult> ValidateAsync(Comment comment)
        {
            return Task.FromResult(Validate(comment.Content));
        }

        private DomainResult Validate(string content)
        {
            return content.Length > maxLength
                ? DomainResult.Error($"Content length is more than {maxLength} characters")
                : DomainResult.Success();
        }
    }
}
