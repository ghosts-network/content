using System;

namespace GhostNetwork.Publications.Comments
{
    public class Comment
    {
        public Comment(string id, string content, DateTimeOffset createdOn, string publicationId, string replyCommentId, string authorId)
        {
            Id = id;
            Content = content;
            CreatedOn = createdOn;
            PublicationId = publicationId;
            ReplyCommentId = replyCommentId;
            AuthorId = authorId;
        }

        public string Id { get; }

        public string Content { get; }

        public string PublicationId { get; }

        public string AuthorId { get; }

        public DateTimeOffset CreatedOn { get; }

        public string ReplyCommentId { get; }
    }
}
