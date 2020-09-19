using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GhostNetwork.Publications.Domain
{
    public class PublicationService : IPublicationService
    {
        private readonly ILengthValidator lengthValidator;
        private readonly PublicationBuilder publicationBuilder;
        private readonly IPublicationStorage publicationStorage;

        public PublicationService(ILengthValidator lengthValidator, PublicationBuilder publicationBuilder, IPublicationStorage publicationStorage)
        {
            this.lengthValidator = lengthValidator;
            this.publicationBuilder = publicationBuilder;
            this.publicationStorage = publicationStorage;
        }

        public async Task<string> CreateAsync(string text)
        {
            if (lengthValidator.Validate(text))
            {
                var publication = publicationBuilder.Build(text);
                var id = await publicationStorage.InsertOneAsync(publication);
                return id;
            }

            return null;
        }

        public async Task<Publication> FindOneByIdAsync(string id)
        {
            if (id != null)
            {
                var publication = await publicationStorage.FindOneByIdAsync(id);
                return publication;
            }

            return null;
        }

        public async Task<IEnumerable<Publication>> FindManyAsync(int skip, int take, IEnumerable<string> tags)
        {
            var publications = await publicationStorage.FindManyAsync(skip, take, tags);
            return publications;
        }
<<<<<<< HEAD
=======

        public async Task<bool> UpdateOneAsync(string id, string text)
        {
            var publications = publicationBuilder.Build(text);

            var update = await publicationStorage.UpdateOneAsync(id, publications);

            return update;
        }
>>>>>>> df7f1acc563a0080ff888a2fa75dda4682842d8f
    }
}
