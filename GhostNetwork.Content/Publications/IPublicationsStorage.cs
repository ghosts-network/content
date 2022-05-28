using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Content.Publications
{
    public interface IPublicationsStorage
    {
        Task<Publication> FindOneByIdAsync(string id);

        Task<string> InsertOneAsync(Publication publication);

        Task<IReadOnlyCollection<Publication>> FindManyAsync(IEnumerable<string> tags, Ordering order, Pagination pagination);

        Task UpdateOneAsync(Publication publication);

        Task DeleteOneAsync(string id);

        Task<IReadOnlyCollection<Publication>> FindManyByAuthorAsync(Guid authorId, Ordering order, Pagination pagination);

        Task UpdateAuthorAsync(Guid authorId, string fullName, string avatarUrl);
    }
}
