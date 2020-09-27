using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain
{
    public interface ICommentStorage
    {
        Task<Comment> FindOneByIdAsync(string id);

        Task<string> InsertOneAsync(Comment publication);

        Task<IEnumerable<Comment>> FindManyAsync(string publicationId, int skip, int take);

        Task<bool> FindCommentInPublicationById(string commentId, string publicationId);
    }
}
