using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain
{
    public interface ICommentsService
    {
        Task<(DomainResult,string)> CreateAsync(string publicationId, string text, string replyCommentId);

        Task<Comment> FindOneByIdAsync(string id);

        Task<IEnumerable<Comment>> FindManyAsync(string publicationId, int skip, int take);
    }
}
