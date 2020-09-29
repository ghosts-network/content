using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain
{
    public class ForbiddenWordsValidator : IPublicationValidator
    {
        private readonly IEnumerable<ForbiddenWordModel> forbidden;

        public ForbiddenWordsValidator(IEnumerable<ForbiddenWordModel> forbiddenWords)
        {
            forbidden = forbiddenWords;
        }

        public Task<DomainResult> ValidateAsync(PublicationContext context)
        {
            foreach (var s in forbidden.Select(x => x.ForbiddenWord))
            {
                if (context.Content.Contains(s))
                {
                    return Task.FromResult(DomainResult.Error("Content contains forbidden words"));
                }
            }

            return Task.FromResult(DomainResult.Successed());
        }
    }
}
