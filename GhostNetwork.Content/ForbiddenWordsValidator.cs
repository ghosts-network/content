using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;
using GhostNetwork.Content.Comments;
using GhostNetwork.Content.Publications;

namespace GhostNetwork.Content
{
    public class ForbiddenWordsValidator : IValidator<Publication>,
        IValidator<Comment>
    {
        private readonly IEnumerable<string> forbiddenWords;

        public ForbiddenWordsValidator(IEnumerable<string> forbiddenWords)
        {
            this.forbiddenWords = forbiddenWords;
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
            return forbiddenWords.Any(content.Contains)
                ? DomainResult.Error("Content contains forbidden words")
                : DomainResult.Success();
        }
    }
}
