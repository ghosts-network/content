using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain
{
    public interface IPublicationService
    {
        Task<string> CreateAsync(string text);

        Task<Publication> FindOneByIdAsync(string id);

        Task<IEnumerable<Publication>> FindManyAsync(int skip, int take, IEnumerable<string> tags);

        Task<bool> UpdateOneAsync(string id, string text);
    }
}
