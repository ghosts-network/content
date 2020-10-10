using System.Collections.Generic;
using System.Threading.Tasks;
using GhostNetwork.Publications.Comments;

namespace GhostNetwork.Publications
{
    public interface IPublicationService
    {
        Task<(DomainResult, string)> CreateAsync(string text);

        Task<Publication> FindOneByIdAsync(string id);

        Task<IEnumerable<Publication>> FindManyAsync(int skip, int take, IEnumerable<string> tags);

        Task<DomainResult> UpdateOneAsync(string id, string text);

        Task<DomainResult> DeleteOneAsync(string id);
    }

    public class PublicationService : IPublicationService
    {
        private readonly IValidator<PublicationContext> validator;
        private readonly PublicationBuilder publicationBuilder;
        private readonly IPublicationsStorage publicationStorage;
        private readonly ICommentsStorage commentStorage;

        public PublicationService(
            IValidator<PublicationContext> validator,
            PublicationBuilder publicationBuilder,
            IPublicationsStorage publicationStorage,
            ICommentsStorage commentStorage)
        {
            this.validator = validator;
            this.publicationBuilder = publicationBuilder;
            this.publicationStorage = publicationStorage;
            this.commentStorage = commentStorage;
        }

        public async Task<(DomainResult, string)> CreateAsync(string text)
        {
            var content = new PublicationContext(text);
            var result = await validator.ValidateAsync(content);

            if (!result.Success)
            {
                return (result, null);
            }

            var publication = publicationBuilder.Build(text);
            var id = await publicationStorage.InsertOneAsync(publication);

            return (result, id);
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

        public async Task<DomainResult> UpdateOneAsync(string id, string text)
        {
            var content = new PublicationContext(text);
            var result = await validator.ValidateAsync(content);

            if (!result.Success)
            {
                return result;
            }

            var publications = publicationBuilder.Build(text);
            await publicationStorage.UpdateOneAsync(id, publications);

            return DomainResult.Successed();
        }

        public async Task<DomainResult> DeleteOneAsync(string id)
        {
            await commentStorage.DeleteByPublicationAsync(id);
            await publicationStorage.DeleteOneAsync(id);

            return DomainResult.Successed();
        }
    }
}
