using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Publications
{
    public interface IPublicationsStorage
    {
        Task<Publication> FindOneByIdAsync(string id);

        Task<string> InsertOneAsync(Publication publication);

        Task<(IEnumerable<Publication>, long)> FindManyAsync(int skip, int take, IEnumerable<string> tags);

        Task UpdateOneAsync(Publication publication);

        Task DeleteOneAsync(string id);
    }
}
