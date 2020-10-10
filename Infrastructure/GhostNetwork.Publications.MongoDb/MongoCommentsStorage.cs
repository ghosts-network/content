using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Publications.Comments;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GhostNetwork.Publications.MongoDb
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
                PublicationId = comment.PublicationId,
                ReplyCommentId = comment.ReplyCommentId,
                AuthorId = comment.AuthorId
            };

            await context.Comments.InsertOneAsync(entity);

            return entity.Id.ToString();
        }

        public async Task<(IEnumerable<Comment>, long)> FindManyAsync(string publicationId, int skip, int take)
        {
            var filter = Builders<CommentEntity>.Filter.Eq(x => x.PublicationId, publicationId);

            var totalCount = await context.Comments
                .Find(filter)
                .CountDocumentsAsync();

            var entities = await context.Comments
                .Find(filter)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();

            return (entities
                .Select(ToDomain)
                .ToList(), totalCount);
        }

        public async Task<bool> IsCommentInPublicationAsync(string commentId, string publicationId)
        {
            if (!ObjectId.TryParse(commentId, out var id))
            {
                return false;
            }

            var filter = Builders<CommentEntity>.Filter.Eq(x => x.PublicationId, publicationId) &
                         Builders<CommentEntity>.Filter.Eq(x => x.Id, id);

            return await context.Comments.Find(filter).AnyAsync();
        }

        public async Task DeleteByPublicationAsync(string publicationId)
        {
            var filter = Builders<CommentEntity>.Filter.Eq(x => x.PublicationId, publicationId);

            await context.Comments.DeleteManyAsync(filter);
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

        private static Comment ToDomain(CommentEntity entity)
        {
            return new Comment(
                entity.Id.ToString(),
                entity.Content,
                DateTimeOffset.FromUnixTimeMilliseconds(entity.CreateOn),
                entity.PublicationId,
                entity.ReplyCommentId,
                entity.AuthorId);
        }
    }
}
