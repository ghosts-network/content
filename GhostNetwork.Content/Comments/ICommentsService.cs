using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;

namespace GhostNetwork.Content.Comments
{
    public interface ICommentsService
    {
        Task<Comment> GetByIdAsync(string id);

        Task<(IEnumerable<Comment>, long)> SearchAsync(string key, int skip, int take);

        Task<(DomainResult, string)> CreateAsync(string key, string text, string replyCommentId, UserInfo author);

        Task<Dictionary<string, FeaturedInfo>> SearchFeaturedAsync(IEnumerable<string> ids);

        Task DeleteAsync(string id);

        Task DeleteByKeyAsync(string key);
    }

    public class CommentsService : ICommentsService
    {
        private readonly ICommentsStorage commentStorage;
        private readonly IValidator<CommentContext> validator;

        public CommentsService(
            ICommentsStorage commentStorage,
            IValidator<CommentContext> validator)
        {
            this.commentStorage = commentStorage;
            this.validator = validator;
        }

        public Task<Comment> GetByIdAsync(string id)
        {
            return commentStorage.FindOneByIdAsync(id);
        }

        public Task<(IEnumerable<Comment>, long)> SearchAsync(string publicationId, int skip, int take)
        {
            return commentStorage.FindManyAsync(publicationId, skip, take);
        }

        public Task<Dictionary<string, FeaturedInfo>> SearchFeaturedAsync(IEnumerable<string> keys)
        {
            return commentStorage.FindFeaturedAsync(keys);
        }

        public async Task<(DomainResult, string)> CreateAsync(string key, string text, string replyId, UserInfo author)
        {
            var result = await validator.ValidateAsync(new CommentContext(text, replyId));
            if (!result.Successed)
            {
                return (result, null);
            }

            var comment = Comment.New(text, key, replyId, author);
            var id = await commentStorage.InsertOneAsync(comment);

            return (DomainResult.Success(), id);
        }

        public Task DeleteAsync(string id)
        {
            return commentStorage.DeleteOneAsync(id);
        }

        public Task DeleteByKeyAsync(string key)
        {
            return commentStorage.DeleteByKeyAsync(key);
        }
    }
}
