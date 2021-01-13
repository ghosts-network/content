using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;

namespace GhostNetwork.Publications.Comments
{
    public interface ICommentsService
    {
        Task<Comment> GetByIdAsync(string id);

        Task<(IEnumerable<Comment>, long)> SearchAsync(string publicationId, int skip, int take);

        Task<(DomainResult, string)> CreateAsync(string publicationId, string text, string replyCommentId, UserInfo author);

        Task DeleteAsync(string id);

        Task<Dictionary<string, IEnumerable<Comment>>> FindCommentsByPublicationsAsync(string[] ids, Ordering order);
    }

    public class CommentsService : ICommentsService
    {
        private readonly ICommentsStorage commentStorage;
        private readonly IPublicationsStorage publicationsStorage;
        private readonly IValidator<CommentContext> validator;

        public CommentsService(
            ICommentsStorage commentStorage,
            IPublicationsStorage publicationsStorage,
            IValidator<CommentContext> validator)
        {
            this.commentStorage = commentStorage;
            this.publicationsStorage = publicationsStorage;
            this.validator = validator;
        }

        public async Task<(DomainResult, string)> CreateAsync(string publicationId, string text, string replyCommentId, UserInfo author)
        {
            var publication = await publicationsStorage.FindOneByIdAsync(publicationId);
            if (publication == null)
            {
                return (DomainResult.Error("Publication not found"), null);
            }

            if (replyCommentId == null || await commentStorage.IsCommentInPublicationAsync(replyCommentId, publicationId))
            {
                var result = await validator.ValidateAsync(new CommentContext(text));
                if (!result.Successed)
                {
                    return (result, null);
                }

                var comment = new Comment(default, text, DateTimeOffset.UtcNow, publicationId, replyCommentId, author);

                return (result, await commentStorage.InsertOneAsync(comment));
            }

            return (DomainResult.Error("Comment id not found"), null);
        }

        public Task<Comment> GetByIdAsync(string id)
        {
            return commentStorage.FindOneByIdAsync(id);
        }

        public async Task<(IEnumerable<Comment>, long)> SearchAsync(string publicationId, int skip, int take)
        {
            return await commentStorage.FindManyAsync(publicationId, skip, take);
        }

        public async Task DeleteAsync(string id)
        {
            await commentStorage.DeleteOneAsync(id);
        }

        public async Task<Dictionary<string, IEnumerable<Comment>>> FindCommentsByPublicationsAsync(string[] ids, Ordering order)
        {
            return await commentStorage.FindCommentsByPublicationsAsync(ids, order);
        }
    }
}
