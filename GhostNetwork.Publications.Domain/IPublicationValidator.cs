using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain
{
    public interface IPublicationValidator
    {
        Task<DomainResult> ValidateAsync(PublicationContext context);
    }
}