using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;
using GhostNetwork.Publications.Comments;

namespace GhostNetwork.Publications
{
    public interface IPublicationService
    {
        Task<Publication> GetByIdAsync(string id);

        Task<(IEnumerable<Publication>, long)> SearchAsync(int skip, int take, IEnumerable<string> tags);

        Task<(DomainResult, string)> CreateAsync(string text, string authorId);

        Task<DomainResult> UpdateAsync(string id, string text);

        Task DeleteAsync(string id);
    }

    public class PublicationService : IPublicationService
    {
        private readonly IValidator<PublicationContext> validator;
        private readonly IPublicationsStorage publicationStorage;
        private readonly ICommentsStorage commentStorage;
        private readonly IHashTagsFetcher hashTagsFetcher;

        public PublicationService(
            IValidator<PublicationContext> validator,
            IPublicationsStorage publicationStorage,
            ICommentsStorage commentStorage,
            IHashTagsFetcher hashTagsFetcher)
        {
            this.validator = validator;
            this.publicationStorage = publicationStorage;
            this.commentStorage = commentStorage;
            this.hashTagsFetcher = hashTagsFetcher;
        }

        public async Task<Publication> GetByIdAsync(string id)
        {
            return await publicationStorage.FindOneByIdAsync(id);
        }

        public async Task<(IEnumerable<Publication>, long)> SearchAsync(int skip, int take, IEnumerable<string> tags)
        {
            return await publicationStorage.FindManyAsync(skip, take, tags);
        }

        public async Task<(DomainResult, string)> CreateAsync(string text, string authorId)
        {
            var content = new PublicationContext(text);
            var result = await validator.ValidateAsync(content);

            if (!result.Successed)
            {
                return (result, null);
            }

            var publication = Publication.New(text, authorId, hashTagsFetcher.Fetch);
            var id = await publicationStorage.InsertOneAsync(publication);

            return (result, id);
        }

        public async Task<DomainResult> UpdateAsync(string id, string text)
        {
            var content = new PublicationContext(text);
            var result = await validator.ValidateAsync(content);

            if (!result.Successed)
            {
                return result;
            }

            var publication = await publicationStorage.FindOneByIdAsync(id);

            publication.Update(text, hashTagsFetcher.Fetch);

            await publicationStorage.UpdateOneAsync(publication);

            return DomainResult.Success();
        }

        public async Task DeleteAsync(string id)
        {
            await commentStorage.DeleteByPublicationAsync(id);
            await publicationStorage.DeleteOneAsync(id);
        }
    }
}
