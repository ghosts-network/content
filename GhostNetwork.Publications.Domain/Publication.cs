using System;

namespace GhostNetwork.Publications.Domain
{
    public class Publication
    {
        public Publication(string id, string content, DateTimeOffset createdOn)
        {
            Id = id;
            Content = content;
            CreatedOn = createdOn;
        }

        public string Id { get; }

        public string Content { get; }

        public DateTimeOffset CreatedOn { get; }

        public static Publication New(string content)
        {
            return new Publication(string.Empty, content, DateTimeOffset.Now);
        }
    }
}