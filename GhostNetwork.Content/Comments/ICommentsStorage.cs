using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Content.Comments
{
    public interface ICommentsStorage
    {
        Task<Comment> FindOneByIdAsync(string id);

        Task<(IEnumerable<Comment>, long)> FindManyAsync(string key, int skip, int take);

        Task<IEnumerable<Comment>> FindManyByCommentReplyIdAsync(string commentId, int skip, int take);

        Task<Dictionary<string, FeaturedInfo>> FindFeaturedAsync(IEnumerable<string> keys);

        Task<string> InsertOneAsync(Comment comment);

        Task UpdateOneAsync(string commentId, string content);

        Task UpdateAuthorAsync(Guid authorId, string fullName, string avatarUrl);

        Task DeleteByKeyAsync(string key);

        Task DeleteOneAsync(string commentId);
    }
}
