using System;

namespace GhostNetwork.Publications.Domain
{
    public class Comment
    {
        public Comment(string id, string content, DateTimeOffset createdOn, string publicationId, string replyCommentId)
        {
            Id = id;
            Content = content;
            CreatedOn = createdOn;
            PublicationId = publicationId;
            ReplyCommentId = replyCommentId;
        }

        public string Id { get; }

        public string Content { get; }

        public string PublicationId { get; }

        public DateTimeOffset CreatedOn { get; }

        public string ReplyCommentId { get; }
    }
}
