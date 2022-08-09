﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Content.Comments;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GhostNetwork.Content.MongoDb
{
    public class MongoCommentsStorage : ICommentsStorage
    {
        private readonly MongoDbContext context;

        public MongoCommentsStorage(MongoDbContext context)
        {
            this.context = context;
        }

        public async Task<Comment> FindOneByIdAsync(string id)
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

            await context.Comments
                .UpdateOneAsync(filter, update);
        }

        public async Task UpdateAuthorAsync(Guid authorId, string fullName, string avatarUrl)
        {
            var filter = Builders<CommentEntity>.Filter.Where(x => x.Author.Id == authorId);

            var update = Builders<CommentEntity>.Update
                .Set(p => p.Author.FullName, fullName)
                .Set(p => p.Author.AvatarUrl, avatarUrl);

            await context.Comments.UpdateManyAsync(filter, update);
        }

        public async Task<(IReadOnlyCollection<Comment>, long)> FindManyAsync(string key, int skip, int take, string cursor, Ordering order)
        {
            var filter = Builders<CommentEntity>.Filter.Eq(x => x.Key, key) & Builders<CommentEntity>.Filter.Eq(x => x.ReplyCommentId, null);

            var sorting = order switch
            {
                Ordering.Desc => Builders<CommentEntity>.Sort.Descending(x => x.CreateOn),
                _ => Builders<CommentEntity>.Sort.Ascending(x => x.CreateOn)
            };

            var totalCount = await context.Comments
                .Find(filter)
                .CountDocumentsAsync();

            if (cursor != null)
            {
                filter &= order switch
                {
                    Ordering.Desc => Builders<CommentEntity>.Filter.Lt(x => x.Id, ObjectId.Parse(cursor)),
                    _ => Builders<CommentEntity>.Filter.Gt(x => x.Id, ObjectId.Parse(cursor))
                };
            }

            var entities = await context.Comments
                .Find(filter)
                .Sort(sorting)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();

            var repliesDict = await FindFeaturedAsync(entities.Select(x => x.Id.ToString()), 3);

            var result = entities.Select(entity =>
            {
                repliesDict.TryGetValue(entity.Id.ToString(), out var replies);

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

        public async Task<Dictionary<string, FeaturedInfo>> FindFeaturedAsync(IReadOnlyCollection<string> keys)
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
                .Match(Builders<CommentEntity>.Filter.In(x => x.Key, keys))
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

        public async Task<Dictionary<string, CommentsShort>> FindFeaturedAsync(IEnumerable<string> ids, int take)
        {
            var group = new BsonDocument
            {
                {
                    "_id", "$replyId"
                },
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
                                take
                            })
                        }
                    }
                },
                { "count", "$count" }
            };

            var listComments = await context.Comments
                .Aggregate()
                .Match(Builders<CommentEntity>.Filter.In(x => x.ReplyCommentId, ids))
                .Sort(Builders<CommentEntity>.Sort.Ascending(x => x.CreateOn))
                .Group<FeaturedInfoEntity>(group)
                .Project<FeaturedInfoEntity>(slice.ToBsonDocument())
                .ToListAsync();

            return listComments.ToDictionary(key => key.Id, value => new CommentsShort(value.Comments.Select(c => ToDomain(c)), value.TotalCount));
        }

        private static Comment ToDomain(CommentEntity entity, CommentsShort replies = null)
        {
            return new Comment(
                entity.Id.ToString(),
                entity.Content,
                DateTimeOffset.FromUnixTimeMilliseconds(entity.CreateOn),
                entity.Key,
                entity.ReplyCommentId,
                (UserInfo)entity.Author,
                replies);
        }
    }
}
