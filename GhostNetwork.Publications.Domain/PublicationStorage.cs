using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain
{
    public class PublicationStorage : IPublicationStorage
    {
        private readonly IList<Publication> publications = new List<Publication>();

        public Task<Publication> FindOneByIdAsync(string id)
        {
            return Task.FromResult(publications.FirstOrDefault(p => p.Id == id));
        }

        public Task<string> InsertOneAsync(Publication publication)
        {
            publication = new Publication(Guid.NewGuid().ToString(), publication.Content);
            publications.Add(publication);

            return Task.FromResult(publication.Id);
        }
    }
}