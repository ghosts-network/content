using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Publications.Comments
{
    public interface ICommentsStorage
    {
        Task<Comment> FindOneByIdAsync(string id);

        Task<IEnumerable<Comment>> FindManyAsync(string publicationId, int skip, int take);

        Task<bool> IsCommentInPublicationAsync(string commentId, string publicationId);

        Task<string> InsertOneAsync(Comment publication);

        Task DeleteByPublicationAsync(string publicationId);

        Task DeleteOneAsync(string commentId);
    }
}
