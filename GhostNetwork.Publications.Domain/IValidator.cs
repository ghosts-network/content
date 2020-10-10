using System.Threading.Tasks;

namespace GhostNetwork.Publications
{
    public interface IValidator<in T>
    {
        Task<DomainResult> ValidateAsync(T context);
    }
}
