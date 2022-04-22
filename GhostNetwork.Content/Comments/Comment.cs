using System;
using System.Collections.Generic;
using System.Linq;

namespace GhostNetwork.Content.Comments
{
    public class Comment
    {
        public Comment(string id, string content, DateTimeOffset createdOn, string key, string replyCommentId, UserInfo author, CommentsShort replies = null)
        {
            Id = id;
            Content = content;
            CreatedOn = createdOn;
            Key = key;
            ReplyCommentId = replyCommentId;
            Author = author;
            Replies = replies;
        }

        public string Id { get; }

        public string Content { get; }

        public string Key { get; }

        public UserInfo Author { get; }

        public DateTimeOffset CreatedOn { get; }

        public string? ReplyCommentId { get; }

        public CommentsShort Replies { get; }

        public static Comment New(string text, string key, string replyId, UserInfo author)
        {
            return new Comment(default, text, DateTimeOffset.UtcNow, key, replyId, author);
        }
    }

    public class CommentsShort
    {
        public CommentsShort(IEnumerable<Comment> topComments, long totalCount)
        {
            TopComments = topComments;
            TotalCount = totalCount;
        }

        public IEnumerable<Comment> TopComments { get; }

        public long TotalCount { get; }
    }
}