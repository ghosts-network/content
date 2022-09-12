using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace GhostNetwork.Content.MongoDb
{
    public class MongoDbContext
    {
        private const string PublicationsCollection = "publications";
        private const string CommentsCollection = "comments";
        private readonly IMongoDatabase database;

        static MongoDbContext()
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        }

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

        public async Task MigrateGuidAsync()
        {
            var publications = await database.GetCollection<MigrationPublication>(PublicationsCollection)
                .Find(Builders<MigrationPublication>.Filter.Not(Builders<MigrationPublication>.Filter.Type(p => p.Author.Id, BsonType.String)))
                .ToListAsync();
            foreach (var publication in publications)
            {
                await database.GetCollection<MigrationPublication>(PublicationsCollection).UpdateOneAsync(
                    Builders<MigrationPublication>.Filter.Eq(p => p.Id, publication.Id),
                    Builders<MigrationPublication>.Update.Set("author._id", publication.Author.Id));
            }

            var comments = await database.GetCollection<MigrationComment>(CommentsCollection)
                .Find(Builders<MigrationComment>.Filter.Not(Builders<MigrationComment>.Filter.Type(p => p.Author.Id, BsonType.String)))
                .ToListAsync();
            foreach (var comment in comments)
            {
                await database.GetCollection<MigrationComment>(CommentsCollection).UpdateOneAsync(
                    Builders<MigrationComment>.Filter.Eq(p => p.Id, comment.Id),
                    Builders<MigrationComment>.Update.Set("author._id", comment.Author.Id));
            }
        }

        [BsonIgnoreExtraElements]
        private record MigrationPublication
        {
            public ObjectId Id { get; set; }

            [BsonElement("author")]
            public MigrationAuthor Author { get; set; }
        }

        [BsonIgnoreExtraElements]
        private record MigrationComment
        {
            public ObjectId Id { get; set; }

            [BsonElement("author")]
            public MigrationAuthor Author { get; set; }
        }

        [BsonIgnoreExtraElements]
        private record MigrationAuthor
        {
            [BsonId]
            [BsonGuidRepresentation(GuidRepresentation.CSharpLegacy)]
            public Guid Id { get; set; }
        }
    }
}
