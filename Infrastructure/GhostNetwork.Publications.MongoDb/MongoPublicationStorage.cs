using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GhostNetwork.Publications.MongoDb
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
                CreateOn = publication.CreatedOn.ToUnixTimeMilliseconds(),
                Tags = publication.Tags.ToList(),
                UpdateOn = publication.UpdatedOn.ToUnixTimeMilliseconds()
            };

            await context.Publications.InsertOneAsync(entity);

            return entity.Id.ToString();
        }

        public async Task<IEnumerable<Publication>> FindManyAsync(int skip, int take, IEnumerable<string> tags)
        {
            var filter = Builders<PublicationEntity>.Filter.Empty;

            if (tags.Any())
            {
                filter &= Builders<PublicationEntity>.Filter.AnyIn(e => e.Tags, tags);
            }

            var entities = await context.Publications.Find(filter)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();

            return entities.Select(ToDomain);
        }

        public async Task UpdateOneAsync(string id, Publication publication)
        {
            if (!ObjectId.TryParse(id, out var oId))
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
                DateTimeOffset.FromUnixTimeMilliseconds(entity.CreateOn),
                entity.Tags,
                DateTimeOffset.FromUnixTimeMilliseconds(entity.UpdateOn));
        }
    }
}