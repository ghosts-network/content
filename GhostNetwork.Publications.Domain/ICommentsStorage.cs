using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain
{
    public interface ICommentsStorage
    {
        Task<Comment> FindOneByIdAsync(string id);

        Task<string> InsertOneAsync(Comment publication);

        Task<IEnumerable<Comment>> FindManyAsync(string publicationId, int skip, int take);

        Task<bool> IsCommentInPublicationAsync(string commentId, string publicationId);

        Task<bool> DeleteAllCommentsInPublicationAsync(string publicationId);

        Task<bool> DeleteOneAsync(string commentId);
    }
}
