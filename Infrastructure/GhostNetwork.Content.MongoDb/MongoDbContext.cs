using System.Threading.Tasks;
using MongoDB.Driver;

namespace GhostNetwork.Content.MongoDb
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase database;

        public MongoDbContext(IMongoDatabase database)
        {
            this.database = database;
        }

        public IMongoCollection<PublicationEntity> Publications =>
            database.GetCollection<PublicationEntity>("publications");

        public IMongoCollection<CommentEntity> Comments =>
            database.GetCollection<CommentEntity>("comments");

        public IMongoCollection<ReactionEntity> Reactions =>
            database.GetCollection<ReactionEntity>("reactions");

        public async Task ConfigureAsync()
        {
            var publicationAuthorIndex = Builders<PublicationEntity>.IndexKeys.Ascending(publication => publication.Author.Id);
            await Publications.Indexes.CreateOneAsync(new CreateIndexModel<PublicationEntity>(publicationAuthorIndex));

            var commentKeyIndex = Builders<CommentEntity>.IndexKeys.Ascending(comment => comment.Key);
            await Comments.Indexes.CreateOneAsync(new CreateIndexModel<CommentEntity>(commentKeyIndex));

            var commentAuthorIndex = Builders<CommentEntity>.IndexKeys.Ascending(comment => comment.Author.Id);
            await Comments.Indexes.CreateOneAsync(new CreateIndexModel<CommentEntity>(commentAuthorIndex));

            var reactionKeyAuthorIndex = Builders<ReactionEntity>.IndexKeys.Combine(
                Builders<ReactionEntity>.IndexKeys.Ascending(reaction => reaction.Key),
                Builders<ReactionEntity>.IndexKeys.Ascending(reaction => reaction.Author));
            await Reactions.Indexes.CreateOneAsync(new CreateIndexModel<ReactionEntity>(reactionKeyAuthorIndex));
        }
    }
}
