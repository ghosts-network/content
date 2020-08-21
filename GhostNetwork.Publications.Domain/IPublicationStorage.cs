using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain
{
    public interface IPublicationStorage
    {
        Task<Publication> FindOneByIdAsync(string id);

        Task<string> InsertOneAsync(Publication publication);
    }
}