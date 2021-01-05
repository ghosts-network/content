using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Publications.Comments;
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
            if (entity != null)
            {
                var comments = await context.Comments.Find(x => x.PublicationId == entity.Id.ToString()).ToListAsync();
                entity.CommentEntities = comments;
            }

            return entity == null ? null : ToDomain(entity);
        }

        public async Task<string> InsertOneAsync(Publication publication)
        {
            var entity = new PublicationEntity
            {
                Content = publication.Content,
                Tags = publication.Tags.ToList(),
                AuthorId = publication.AuthorId,
                CreateOn = publication.CreatedOn.ToUnixTimeMilliseconds(),
                UpdateOn = publication.UpdatedOn.ToUnixTimeMilliseconds()
            };

            await context.Publications.InsertOneAsync(entity);

            return entity.Id.ToString();
        }

        public async Task<(IEnumerable<Publication>, long)> FindManyAsync(int skip, int take, IEnumerable<string> tags, Ordering order)
        {
            var filter = Builders<PublicationEntity>.Filter.Empty;

            if (tags.Any())
            {
                filter &= Builders<PublicationEntity>.Filter.AnyIn(e => e.Tags, tags);
            }

            var totalCount = await context.Publications.Find(filter)
                .CountDocumentsAsync();

            var sorting = order switch
            {
                Ordering.Desc => Builders<PublicationEntity>.Sort.Descending(x => x.CreateOn),
                _ => Builders<PublicationEntity>.Sort.Ascending(x => x.CreateOn)
            };

            var entities = await context.Publications.Find(filter)
                .Sort(sorting)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();

            var comments = await context.Comments.AsQueryable().ToListAsync();

            foreach (var publication in entities)
            {
                var publicationComments = comments.Where(x => x.PublicationId == publication.Id.ToString()).ToList();
                if (publicationComments.Any())
                {
                    publication.CommentEntities = publicationComments;
                }
                else
                {
                    publication.CommentEntities = new List<CommentEntity>();
                }
            }

            return (entities.Select(ToDomain), totalCount);
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
                entity.AuthorId,
                DateTimeOffset.FromUnixTimeMilliseconds(entity.CreateOn),
                DateTimeOffset.FromUnixTimeMilliseconds(entity.UpdateOn),
                ToListComments(entity.CommentEntities.ToList()));
        }

        private static List<Comment> ToListComments(List<CommentEntity> entities)
        {
            List<Comment> comments = new List<Comment>();
            foreach (var comment in entities)
            {
                var com = new Comment(
                    comment.Id.ToString(),
                    comment.Content,
                    DateTimeOffset.FromUnixTimeMilliseconds(comment.CreateOn),
                    comment.PublicationId,
                    comment.ReplyCommentId,
                    comment.AuthorId);
                comments.Add(com);
            }

            return comments;
        }
    }
}