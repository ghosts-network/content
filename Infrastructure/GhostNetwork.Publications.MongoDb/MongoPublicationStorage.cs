using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Publications.Domain;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GhostNetwork.Publications.MongoDb
{
    public class MongoPublicationStorage : IPublicationStorage
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

            return entity == null ? null : new Publication(
                entity.Id.ToString(),
                entity.Content,
                DateTimeOffset.FromUnixTimeMilliseconds(entity.CreateOn),
                entity.Tags,
                DateTimeOffset.FromUnixTimeMilliseconds(entity.UpdateOn),
                entity.IsUpdated);
        }

        public async Task<string> InsertOneAsync(Publication publication)
        {
            var entity = new PublicationEntity
            {
                Content = publication.Content,
                CreateOn = publication.CreatedOn.ToUnixTimeMilliseconds(),
                Tags = publication.Tags.ToList(),
                UpdateOn = publication.UpdatedOn.ToUnixTimeMilliseconds(),
                IsUpdated = publication.IsUpdated
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

            return entities.Select(entity => new Publication(
                entity.Id.ToString(),
                entity.Content,
                DateTimeOffset.FromUnixTimeMilliseconds(entity.CreateOn),
                entity.Tags,
                DateTimeOffset.FromUnixTimeMilliseconds(entity.UpdateOn),
                entity.IsUpdated));
        }

        public async Task<bool> UpdateOneAsync(string id, Publication publication)
        {
            var filter = Builders<PublicationEntity>.Filter.Eq(p => p.Id, new ObjectId(id));

            var update = Builders<PublicationEntity>.Update.Set(s => s.Content, publication.Content)
                .Set(s => s.Tags, publication.Tags.ToList())
                .Set(s => s.UpdateOn, publication.UpdatedOn.ToUnixTimeMilliseconds())
                .Set(s => s.IsUpdated, publication.IsUpdated);

            UpdateResult updateResult = await context.Publications.UpdateOneAsync(filter, update);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
    }
}