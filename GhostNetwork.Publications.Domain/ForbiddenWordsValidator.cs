using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;
using GhostNetwork.Publications.Comments;

namespace GhostNetwork.Publications
{
    public class ForbiddenWordsValidator : IValidator<PublicationContext>,
        IValidator<CommentContext>
    {
        private readonly IEnumerable<string> forbiddenWords;

        public ForbiddenWordsValidator(IEnumerable<string> forbiddenWords)
        {
            this.forbiddenWords = forbiddenWords;
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
            return forbiddenWords.Any(content.Contains)
                ? DomainResult.Error("Content contains forbidden words")
                : DomainResult.Success();
        }
    }
}
