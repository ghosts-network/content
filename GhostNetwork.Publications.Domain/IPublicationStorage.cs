using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain
{
    public interface IPublicationStorage
    {
        Task<Publication> FindOneByIdAsync(string id);

        Task<string> InsertOneAsync(Publication publication);

        Task<IEnumerable<Publication>> FindManyAsync(int skip, int take, IEnumerable<string> tags);
    }
}
