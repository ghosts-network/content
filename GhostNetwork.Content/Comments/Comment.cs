using System;

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
    }
}
