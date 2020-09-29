using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain
{
    public sealed class PublicationValidatorsContainer : IPublicationValidator
    {
        private readonly IEnumerable<IPublicationValidator> validators;

        public PublicationValidatorsContainer(params IPublicationValidator[] validators)
        {
            this.validators = validators;
        }

        public async Task<DomainResult> ValidateAsync(PublicationContext context)
        {
            var errors = new List<DomainError>();
            foreach (var validator in validators)
            {
                var result = await validator.ValidateAsync(context);
                if (!result.Success)
                {
                    errors.AddRange(result.Errors);
                }
            }

            return errors.Any()
                ? DomainResult.Error(errors)
                : DomainResult.Successed();
        }
    }
}