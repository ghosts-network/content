﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;

namespace GhostNetwork.Content.Comments
{
    public interface ICommentsService
    {
        Task<Comment> GetByIdAsync(string id);

        Task<(IReadOnlyCollection<Comment>, long)> SearchAsync(string key, int skip, int take, string cursor, Ordering order);

        Task<(DomainResult, string)> CreateAsync(string key, string text, string replyCommentId, UserInfo author);

        Task<DomainResult> UpdateAsync(string commentId, string content);

        Task<Dictionary<string, FeaturedInfo>> SearchFeaturedAsync(IReadOnlyCollection<string> keys);

        Task DeleteAsync(string id);

        Task DeleteByKeyAsync(string key);
    }

    public class CommentsService : ICommentsService
    {
        private readonly ICommentsStorage commentStorage;
        private readonly IValidator<Comment> validator;

        public CommentsService(
            ICommentsStorage commentStorage,
            IValidator<Comment> validator)
        {
            this.commentStorage = commentStorage;
            this.validator = validator;
        }

        public Task<Comment> GetByIdAsync(string id)
        {
            return commentStorage.FindOneByIdAsync(id);
        }

        public Task<(IReadOnlyCollection<Comment>, long)> SearchAsync(string key, int skip, int take, string cursor, Ordering order)
        {
            return commentStorage.FindManyAsync(key, skip, take, cursor, order);
        }

        public Task<Dictionary<string, FeaturedInfo>> SearchFeaturedAsync(IReadOnlyCollection<string> keys)
        {
            return commentStorage.FindFeaturedAsync(keys);
        }

        public async Task<(DomainResult, string)> CreateAsync(string key, string text, string replyId, UserInfo author)
        {
            var comment = Comment.New(text, key, replyId, author);
            var result = await validator.ValidateAsync(comment);

            if (!result.Successed)
            {
                return (result, null);
            }

            var id = await commentStorage.InsertOneAsync(comment);

            return (DomainResult.Success(), id);
        }

        public async Task<DomainResult> UpdateAsync(string commentId, string content)
        {
            var actualComment = await GetByIdAsync(commentId);
            var updatedComment = new Comment(
                actualComment.Id,
                content,
                actualComment.CreatedOn,
                actualComment.Key,
                actualComment.ReplyCommentId,
                actualComment.Author);

            var result = await validator.ValidateAsync(updatedComment);

            if (!result.Successed)
            {
                return result;
            }

            await commentStorage.UpdateOneAsync(commentId, content);

            return DomainResult.Success();
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
