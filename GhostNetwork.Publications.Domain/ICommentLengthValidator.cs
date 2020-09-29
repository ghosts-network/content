using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain
{
    public interface ICommentLengthValidator
    {
        Task<DomainResult> ValidateAsync(CommentContext content);
    }
}
