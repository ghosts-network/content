using GhostNetwork.Content.Comments;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhostNetwork.Content.MongoDb
{
    public class MongoCommentsStorage : ICommentsStorage
    {
        private readonly MongoDbContext context;

        public MongoCommentsStorage(MongoDbContext context)
        {
            this.context = context;
        }

        public async Task<Comment?> FindOneByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var oId))
            {
                return null;
            }

            var filter = Builders<CommentEntity>.Filter.Eq(p => p.Id, oId);

            var entity = await context.Comments
                .Find(filter)
                .FirstOrDefaultAsync();

            return entity == null ? null : ToDomain(entity);
        }

        public async Task<string> InsertOneAsync(Comment comment)
        {
            var entity = new CommentEntity
            {
                Content = comment.Content,
                CreateOn = comment.CreatedOn.ToUnixTimeMilliseconds(),
                Key = comment.Key,
                ReplyCommentId = comment.ReplyCommentId,
                Author = (UserInfoEntity)comment.Author
            };

            await context.Comments.InsertOneAsync(entity);

            return entity.Id.ToString();
        }

        public async Task UpdateOneAsync(string commentId, string content)
        {
            if (!ObjectId.TryParse(commentId, out var oId))
            {
                return;
            }

            var filter = Builders<CommentEntity>.Filter.Eq(x => x.Id, oId);
            var update = Builders<CommentEntity>.Update.Set(x => x.Content, content);

            var updateResult = await context.Comments
                .UpdateOneAsync(filter, update);

            return;
        }

        public async Task UpdateAuthorAsync(Guid authorId, string fullName, string avatarUrl)
        {
            var filter = Builders<CommentEntity>.Filter.Where(x => x.Author.Id == authorId);

            var update = Builders<CommentEntity>.Update
                .Set(p => p.Author.FullName, fullName)
                .Set(p => p.Author.AvatarUrl, avatarUrl);

            await context.Comments.UpdateManyAsync(filter, update);
        }

        public async Task<(IEnumerable<Comment>, long)> FindManyAsync(string key, int skip, int take)
        {
            var filter = Builders<CommentEntity>.Filter.Eq(x => x.Key, key) & Builders<CommentEntity>.Filter.Eq(x => x.ReplyCommentId, null);

            var totalCount = await context.Comments
                .Find(filter)
                .CountDocumentsAsync();

            var entities = await context.Comments
                .Find(filter)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();

            var searchedIds = entities.Select(x => x.Id.ToString());

            var repliesDict = await GetRepliesByManyCommentIdsAsync(searchedIds, take);

            var result = entities.Select(entity =>
            {
                var replies = searchedIds.Contains(entity.Id.ToString())
                    ? repliesDict[entity.Id.ToString()]
                    : Enumerable.Empty<CommentEntity>();

                return ToDomain(entity, replies);
            }).Where(c => string.IsNullOrEmpty(c.ReplyCommentId));

            return (result, totalCount);
        }

        public async Task DeleteOneAsync(string commentId)
        {
            if (!ObjectId.TryParse(commentId, out var oId))
            {
                return;
            }

            var filter = Builders<CommentEntity>.Filter.Eq(x => x.Id, oId);

            await context.Comments.DeleteOneAsync(filter);
        }

        public async Task DeleteByKeyAsync(string key)
        {
            var filter = Builders<CommentEntity>.Filter.Eq(x => x.Key, key);

            await context.Comments.DeleteManyAsync(filter);
        }

        public async Task<Dictionary<string, FeaturedInfo>> FindFeaturedAsync(IEnumerable<string> keys)
        {
            var group = new BsonDocument
            {
                { "_id", "$key" },
                {
                    "comments", new BsonDocument
                    {
                        { "$push", "$$ROOT" }
                    }
                },
                {
                    "count", new BsonDocument("$sum", 1)
                }
            };

            var slice = new BsonDocument
            {
                {
                    "comments", new BsonDocument
                    {
                        {
                            "$slice", new BsonArray(new BsonValue[]
                            {
                                "$comments",
                                3
                            })
                        }
                    }
                },
                { "count", "$count" }
            };

            var listComments = await context.Comments
                .Aggregate()
                .Match(Builders<CommentEntity>.Filter.In(x => x.ReplyCommentId, keys))
                .Sort(Builders<CommentEntity>.Sort.Ascending(x => x.CreateOn))
                .Group<FeaturedInfoEntity>(group)
                .Project<FeaturedInfoEntity>(slice.ToBsonDocument())
                .ToListAsync();

            var dict = listComments
                .ToDictionary(
                    r => r.Id,
                    r => new FeaturedInfo(
                        r.Comments.Select(x => ToDomain(x)),
                        r.TotalCount));

            return keys.ToDictionary(
                    key => key,
                    key => dict.ContainsKey(key)
                        ? dict[key]
                        : new FeaturedInfo(
                            Enumerable.Empty<Comment>(),
                            0));
        }

        private async Task<Dictionary<string, IEnumerable<CommentEntity>>> GetRepliesByManyCommentIdsAsync(IEnumerable<string> keys, int take)
        {
            var filter = Builders<CommentEntity>.Filter.Ne(x => x.ReplyCommentId, null) & Builders<CommentEntity>.Filter.In(x => x.ReplyCommentId, keys);
            var sorting = Builders<CommentEntity>.Sort.Ascending(x => x.CreateOn);

            var replies = await context.Comments
                .Find(filter)
                .Sort(sorting)
                .Limit(take)
                .ToListAsync();

            var groupingReplies = replies.GroupBy(r => r.ReplyCommentId);

            return keys.ToDictionary(
                key => key,
                key => groupingReplies.FirstOrDefault(x => x.Key == key) ?? Enumerable.Empty<CommentEntity>()
            );
        }

        private static Comment ToDomain(CommentEntity entity, IEnumerable<CommentEntity> replies = null)
        {
            return new Comment(
                entity.Id.ToString(),
                entity.Content,
                DateTimeOffset.FromUnixTimeMilliseconds(entity.CreateOn),
                entity.Key,
                entity.ReplyCommentId,
                (UserInfo)entity.Author,
                replies != null ? replies.Select(r => ToDomain(r)) : Enumerable.Empty<Comment>());
        }
    }
}
