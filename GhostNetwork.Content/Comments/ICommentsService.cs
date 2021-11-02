﻿using System;
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

        Task<DomainResult> UpdateAsync(string commentId, string content);

        Task<Dictionary<string, FeaturedInfo>> SearchFeaturedAsync(IEnumerable<string> keys);

        Task DeleteAsync(string id);

        Task DeleteByKeyAsync(string key);
    }

    public class CommentsService : ICommentsService
    {
        private readonly ICommentsStorage commentStorage;
        private readonly IValidator<CommentContext> validator;

        private readonly TimeSpan? allowTimeToEdit;

        public CommentsService(
            ICommentsStorage commentStorage,
            IValidator<CommentContext> validator,
            TimeSpan? allowTimeToEdit = null)
        {
            this.commentStorage = commentStorage;
            this.validator = validator;

            this.allowTimeToEdit = allowTimeToEdit;
        }

        public Task<Comment> GetByIdAsync(string id)
        {
            return commentStorage.FindOneByIdAsync(id);
        }

        public Task<(IEnumerable<Comment>, long)> SearchAsync(string key, int skip, int take)
        {
            return commentStorage.FindManyAsync(key, skip, take);
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

        public async Task<DomainResult> UpdateAsync(string commentId, string content)
        {
            var result = await validator.ValidateAsync(new CommentContext(content));

            if (!result.Successed)
            {
                return result;
            }

            var comment = await GetByIdAsync(commentId);

            if (allowTimeToEdit.HasValue && comment.CreatedOn.Add(allowTimeToEdit.Value) < DateTimeOffset.UtcNow)
            {
                return DomainResult.Error($"Comment cannot update after {allowTimeToEdit.Value.Minutes} minutes after it was created");
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
