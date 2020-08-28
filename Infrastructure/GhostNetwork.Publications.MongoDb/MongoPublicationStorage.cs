using System;
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
            var filter = Builders<PublicationEntity>.Filter.Eq(p => p.Id, new ObjectId(id));
            var entity = await context.Publications.Find(filter).FirstOrDefaultAsync();

            return entity == null ? null : new Publication(
                entity.Id.ToString(),
                entity.Content,
                DateTimeOffset.FromUnixTimeMilliseconds(entity.CreateOn));
        }

        public async Task<string> InsertOneAsync(Publication publication)
        {
            var entity = new PublicationEntity
            {
                Content = publication.Content,
                CreateOn = publication.CreatedOn.ToUnixTimeMilliseconds()
            };
            await context.Publications.InsertOneAsync(entity);

            return entity.Id.ToString();
        }
    }
}