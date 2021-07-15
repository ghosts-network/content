using System;
using System.Collections.Generic;

namespace GhostNetwork.Content.Comments
{
    public class Comment
    {
        public Comment(string id, string content, DateTimeOffset createdOn, string key, string replyCommentId, UserInfo author)
        {
            Id = id;
            Content = content;
            CreatedOn = createdOn;
            Key = key;
            ReplyCommentId = replyCommentId;
            Author = author;
        }

        public string Id { get; }

        public string Content { get; }

        public string Key { get; }

        public UserInfo Author { get; }

        public DateTimeOffset CreatedOn { get; }

        public string ReplyCommentId { get; }

        public static Comment New(string text, string key, string replyId, UserInfo author)
        {
            return new(default, text, DateTimeOffset.UtcNow, key, replyId, author);
        }

        public override bool Equals(object obj)
        {
            return obj is Comment comment &&
                   Id == comment.Id &&
                   Content == comment.Content &&
                   Key == comment.Key &&
                   EqualityComparer<UserInfo>.Default.Equals(Author, comment.Author) &&
                   CreatedOn.Equals(comment.CreatedOn) &&
                   ReplyCommentId == comment.ReplyCommentId;
        }
    }
}
