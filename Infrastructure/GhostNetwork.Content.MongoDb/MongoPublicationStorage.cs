using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Content.Publications;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GhostNetwork.Content.MongoDb
{
    public class MongoPublicationStorage : IPublicationsStorage
    {
        private readonly MongoDbContext context;

        public MongoPublicationStorage(MongoDbContext context)
        {
            this.context = context;
        }

        public async Task<Publication> FindOneByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var oId))
            {
                return null;
            }

            var filter = Builders<PublicationEntity>.Filter.Eq(p => p.Id, oId);
            var entity = await context.Publications.Find(filter).FirstOrDefaultAsync();

            return entity == null ? null : ToDomain(entity);
        }

        public async Task<string> InsertOneAsync(Publication publication)
        {
            var entity = new PublicationEntity
            {
                Content = publication.Content,
                Tags = publication.Tags.ToList(),
                Author = (UserInfoEntity)publication.Author,
                CreateOn = publication.CreatedOn.ToUnixTimeMilliseconds(),
                UpdateOn = publication.UpdatedOn.ToUnixTimeMilliseconds()
            };

            await context.Publications.InsertOneAsync(entity);

            return entity.Id.ToString();
        }

        public async Task<(IReadOnlyCollection<Publication>, long)> FindManyAsync(IEnumerable<string> tags, Ordering order, Pagination pagination)
        {
            var filter = Builders<PublicationEntity>.Filter.Empty;

            if (pagination.Cursor != null)
            {
                filter = order switch
                {
                    Ordering.Desc => Builders<PublicationEntity>.Filter.Lt(x => x.Id, ObjectId.Parse(pagination.Cursor)),
                    _ => Builders<PublicationEntity>.Filter.Gt(x => x.Id, ObjectId.Parse(pagination.Cursor))
                };
            }

            var totalCount = await context.Publications
                .Find(filter)
                .CountDocumentsAsync();

            var sorting = order switch
            {
                Ordering.Desc => Builders<PublicationEntity>.Sort.Descending(x => x.CreateOn),
                _ => Builders<PublicationEntity>.Sort.Ascending(x => x.CreateOn)
            };

            var entities = await context.Publications.Find(filter)
                .Sort(sorting)
                .Skip(pagination.Cursor == null ? pagination.Skip : 0)
                .Limit(pagination.Limit)
                .ToListAsync();

            return (entities.Select(ToDomain).ToList(), totalCount);
        }

        public async Task<(IEnumerable<Publication>, long)> FindManyByAuthorAsync(int skip, int take, Guid authorId, Ordering order)
        {
            var filter = Builders<PublicationEntity>.Filter.Where(x => x.Author.Id == authorId);

            var totalCount = await context.Publications
                .Find(filter)
                .CountDocumentsAsync();

            var sorting = order switch
            {
                Ordering.Desc => Builders<PublicationEntity>.Sort.Descending(x => x.CreateOn),
                _ => Builders<PublicationEntity>.Sort.Ascending(x => x.CreateOn)
            };

            var entities = await context.Publications
                .Find(filter)
                .Sort(sorting)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();

            return (entities.Select(ToDomain), totalCount);
        }

        public async Task UpdateAuthorAsync(Guid authorId, string fullName, string avatarUrl)
        {
            var filter = Builders<PublicationEntity>.Filter.Where(x => x.Author.Id == authorId);

            var update = Builders<PublicationEntity>.Update
                .Set(p => p.Author.FullName, fullName)
                .Set(p => p.Author.AvatarUrl, avatarUrl);

            await context.Publications.UpdateManyAsync(filter, update);
        }

        public async Task UpdateOneAsync(Publication publication)
        {
            if (!ObjectId.TryParse(publication.Id, out var oId))
            {
                return;
            }

            var filter = Builders<PublicationEntity>.Filter.Eq(p => p.Id, oId);

            var update = Builders<PublicationEntity>.Update.Set(s => s.Content, publication.Content)
                .Set(s => s.Tags, publication.Tags.ToList())
                .Set(s => s.UpdateOn, publication.UpdatedOn.ToUnixTimeMilliseconds());

            await context.Publications.UpdateOneAsync(filter, update);
        }

        public async Task DeleteOneAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var oId))
            {
                return;
            }

            var filter = Builders<PublicationEntity>.Filter.Eq(p => p.Id, oId);

            await context.Publications.DeleteOneAsync(filter);
        }

        private static Publication ToDomain(PublicationEntity entity)
        {
            return new Publication(
                entity.Id.ToString(),
                entity.Content,
                entity.Tags,
                (UserInfo)entity.Author,
                DateTimeOffset.FromUnixTimeMilliseconds(entity.CreateOn),
                DateTimeOffset.FromUnixTimeMilliseconds(entity.UpdateOn));
        }
    }
}