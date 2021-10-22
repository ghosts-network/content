using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;

namespace GhostNetwork.Content.Publications
{
    public interface IPublicationService
    {
        Task<Publication> GetByIdAsync(string id);

        Task<(IEnumerable<Publication>, long)> SearchAsync(int skip, int take, IEnumerable<string> tags, Ordering order);

        Task<(DomainResult, string)> CreateAsync(string text, UserInfo author);

        Task<DomainResult> UpdateAsync(string id, string text);

        Task DeleteAsync(string id);

        Task<(IEnumerable<Publication>, long)> SearchByAuthor(int skip, int take, Guid authorId, Ordering order);
    }

    public class PublicationService : IPublicationService
    {
        private readonly IValidator<PublicationContext> validator;
        private readonly IPublicationsStorage publicationStorage;
        private readonly IHashTagsFetcher hashTagsFetcher;
        private readonly IEventBus eventBus;

        public PublicationService(
            IValidator<PublicationContext> validator,
            IPublicationsStorage publicationStorage,
            IHashTagsFetcher hashTagsFetcher,
            IEventBus eventBus)
        {
            this.validator = validator;
            this.publicationStorage = publicationStorage;
            this.hashTagsFetcher = hashTagsFetcher;
            this.eventBus = eventBus;
        }

        public async Task<Publication> GetByIdAsync(string id)
        {
            return await publicationStorage.FindOneByIdAsync(id);
        }

        public async Task<(IEnumerable<Publication>, long)> SearchAsync(int skip, int take, IEnumerable<string> tags, Ordering order)
        {
            return await publicationStorage.FindManyAsync(skip, take, tags, order);
        }

        public async Task<(DomainResult, string)> CreateAsync(string text, UserInfo author)
        {
            var content = new PublicationContext(text);
            var result = await validator.ValidateAsync(content);

            if (!result.Successed)
            {
                return (result, null);
            }

            var publication = Publication.New(text, author, hashTagsFetcher.Fetch);
            var id = await publicationStorage.InsertOneAsync(publication);
            await eventBus.PublishAsync(new CreatedEvent(id, publication.Content, author));

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
            await eventBus.PublishAsync(new CreatedEvent(publication.Id,
                publication.Content,
                publication.Author));

            return DomainResult.Success();
        }

        public async Task DeleteAsync(string id)
        {
            await publicationStorage.DeleteOneAsync(id);
        }

        public async Task<(IEnumerable<Publication>, long)> SearchByAuthor(int skip, int take, Guid authorId, Ordering order)
        {
            return await publicationStorage.FindManyByAuthorAsync(skip, take, authorId, order);
        }
    }
}
