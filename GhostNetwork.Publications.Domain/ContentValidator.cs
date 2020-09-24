using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain.ContentValidation
{
    public class ForbiddenWordsValidator : IPublicationValidator
    {
        private readonly List<ForbiddenWordModel> forbidden;

        public ForbiddenWordsValidator()
        {
            forbidden = new List<ForbiddenWordModel>
            {
                new ForbiddenWordModel {ForbiddenWord = "duck"},
                new ForbiddenWordModel {ForbiddenWord = "dog"},
                new ForbiddenWordModel {ForbiddenWord = "cat"}
            };
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
