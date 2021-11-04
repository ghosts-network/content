using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;
using GhostNetwork.EventBus;

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
        private readonly IValidator<Publication> validator;
        private readonly IPublicationsStorage publicationStorage;
        private readonly IHashTagsFetcher hashTagsFetcher;
        private readonly IEventBus eventBus;

        public PublicationService(
            IValidator<Publication> validator,
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
            var publication = Publication.New(text, author, hashTagsFetcher.Fetch);
            var result = await validator.ValidateAsync(publication);

            if (!result.Successed)
            {
                return (result, null);
            }

            var id = await publicationStorage.InsertOneAsync(publication);

            await eventBus.PublishAsync(new CreatedEvent(id, publication.Content, author));

            return (result, id);
        }

        public async Task<DomainResult> UpdateAsync(string id, string text)
        {
            var publication = await publicationStorage.FindOneByIdAsync(id);
            var result = await validator.ValidateAsync(publication);

            if (!result.Successed)
            {
                return result;
            }

            publication.Update(text, hashTagsFetcher.Fetch);

            await publicationStorage.UpdateOneAsync(publication);

            await eventBus.PublishAsync(new UpdatedEvent(publication.Id,
                publication.Content,
                publication.Author));

            return DomainResult.Success();
        }

        public async Task DeleteAsync(string id)
        {
            var publication = await publicationStorage.FindOneByIdAsync(id);
            await publicationStorage.DeleteOneAsync(id);
            await eventBus.PublishAsync(new DeletedEvent(publication.Id, publication.Author));        
        }

        public async Task<(IEnumerable<Publication>, long)> SearchByAuthor(int skip, int take, Guid authorId, Ordering order)
        {
            return await publicationStorage.FindManyByAuthorAsync(skip, take, authorId, order);
        }
    }
}
