using System;
using System.Collections.Generic;
using GhostNetwork.Publications.Comments;

namespace GhostNetwork.Publications
{
    public class Publication
    {
        public Publication(string id, string content, IEnumerable<string> tags, string authorId, DateTimeOffset createdOn, DateTimeOffset updatedOn, IEnumerable<Comment> comments = null)
        {
            Id = id;
            Content = content;
            Tags = tags;
            AuthorId = authorId;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
            Comments = comments;
        }

        public string Id { get; }

        public string Content { get; private set; }

        public IEnumerable<string> Tags { get; private set; }

        public string AuthorId { get; }

        public DateTimeOffset CreatedOn { get; }

        public DateTimeOffset UpdatedOn { get; private set; }

        public IEnumerable<Comment> Comments { get; set; }

        public bool IsUpdated => CreatedOn != UpdatedOn;

        public static Publication New(string content, string authorId, Func<string, IEnumerable<string>> tagsFetcher)
        {
            var now = DateTimeOffset.UtcNow;

            return new Publication(default, content, tagsFetcher(content), authorId, now, now);
        }

        public Publication Update(string content, Func<string, IEnumerable<string>> tagsFetcher)
        {
            Content = content;
            UpdatedOn = DateTimeOffset.UtcNow;
            Tags = tagsFetcher(content);

            return this;
        }
    }
}