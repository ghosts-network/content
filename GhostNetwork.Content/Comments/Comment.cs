using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GhostNetwork.Content.Comments
{
    public class Comment
    {
        public Comment(string id, string content, DateTimeOffset createdOn, string key, string replyCommentId, UserInfo author, IEnumerable<Comment> replies)
        {
            Id = id;
            Content = content;
            CreatedOn = createdOn;
            Key = key;
            ReplyCommentId = replyCommentId;
            Author = author;
            Replies = replies ?? Enumerable.Empty<Comment>();
        }

        public string Id { get; }

        public string Content { get; }

        public string Key { get; }

        public UserInfo Author { get; }

        public DateTimeOffset CreatedOn { get; }

        public string? ReplyCommentId { get; }

        public IEnumerable<Comment> Replies { get; }

        public static Comment New(string text, string key, string replyId, UserInfo author)
        {
            return new Comment(default, text, DateTimeOffset.UtcNow, key, replyId, author, Enumerable.Empty<Comment>());
        }
    }
}