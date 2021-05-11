using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Content.Reactions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GhostNetwork.Content.MongoDb
{
    public class MongoReactionStorage : IReactionStorage
    {
        private readonly MongoDbContext context;

        public MongoReactionStorage(MongoDbContext context)
        {
            this.context = context;
        }

        public async Task<IDictionary<string, int>> GetStats(string key)
        {
            var filter = Builders<ReactionEntity>.Filter.Eq(p => p.Key, key);

            var result = await context.Reactions
                .Find(filter)
                .ToListAsync();

            return result
                .ToDictionary(x => x.Id.ToString(), x => x.Type)
                .GroupBy(r => r.Value)
                .ToDictionary(rg => rg.Key, rg => rg.Count());
        }

        public async Task<Reaction> GetReactionByAuthorAsync(string key, string author)
        {
            var filter = Builders<ReactionEntity>.Filter.Eq(p => p.Key, key)
                         & Builders<ReactionEntity>.Filter.Eq(p => p.Author, author);

            var reaction = await context.Reactions.Find(filter).FirstOrDefaultAsync();

            return reaction == null ? null : ToDomain(reaction);
        }

        public async Task<IEnumerable<Reaction>> GetReactionsByAuthorAsync(string author, IEnumerable<string> keys)
        {
            var authorFilter = Builders<ReactionEntity>.Filter.Eq(p => p.Author, author);
            var keysFilter = keys.Any() ? Builders<ReactionEntity>.Filter.In(p => p.Key, keys) : FilterDefinition<ReactionEntity>.Empty;

            var reactions = await context.Reactions.Find(authorFilter & keysFilter).ToListAsync();

            return ToDomain(reactions);
        }

        public async Task<IDictionary<string, Dictionary<string, int>>> GetGroupedReactionsAsync(IEnumerable<string> keys)
        {
            var filter = Builders<ReactionEntity>.Filter.In(p => p.Key, keys);

            var reactions = await context.Reactions.Aggregate()
                .Match(filter)
                .Group<GroupedReaction>(new BsonDocument
                {
                    {
                        "_id", new BsonDocument
                        {
                            { "type", "$Type" },
                            { "key", "$Key" }
                        }
                    },
                    {
                        "count", new BsonDocument
                        {
                            { "$sum", 1 }
                        }
                    }
                })
                .ToListAsync();

            return reactions
                .GroupBy(g => g.Key.Key)
                .ToDictionary(k => k.Key, value => value.GroupBy(r => r.Key.Type).ToDictionary(rg => rg.Key, rg => rg.First().Count));
        }

        public async Task UpsertAsync(string key, string author, string type)
        {
            var filter = Builders<ReactionEntity>.Filter.Eq(p => p.Key, key)
                         & Builders<ReactionEntity>.Filter.Eq(p => p.Author, author);

            var update = Builders<ReactionEntity>.Update
                .Set(r => r.Type, type);

            await context.Reactions.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });
        }

        public async Task DeleteByAuthorAsync(string key, string author)
        {
            var filter = Builders<ReactionEntity>.Filter.Eq(p => p.Key, key)
                         & Builders<ReactionEntity>.Filter.Eq(p => p.Author, author);

            await context.Reactions.DeleteOneAsync(filter);
        }

        public async Task DeleteAsync(string key)
        {
            var filter = Builders<ReactionEntity>.Filter.Eq(p => p.Key, key);

            await context.Reactions.DeleteManyAsync(filter);
        }

        private static Reaction ToDomain(ReactionEntity entity)
        {
            return new Reaction(entity.Key, entity.Type);
        }

        private static IEnumerable<Reaction> ToDomain(IEnumerable<ReactionEntity> entities)
        {
            return entities.Select(entity => new Reaction(entity.Key, entity.Type));
        }
    }
}