using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain
{
    public class LengthValidator : IPublicationValidator
    {
        private readonly int? length;

        public LengthValidator(int? length = null)
        {
            this.length = length;
        }

        public Task<DomainResult> ValidateAsync(PublicationContext context)
        {
            if (length == null)
            {
                return Task.FromResult(DomainResult.Successed());
            }

            if (context.Content.Length > length)
            {
                return Task.FromResult(DomainResult.Error($"Content is more than {length.Value} characters"));
            }

            return Task.FromResult(DomainResult.Successed());
        }
    }
}
