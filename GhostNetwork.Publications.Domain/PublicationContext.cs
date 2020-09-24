using System;

namespace GhostNetwork.Publications.Domain
{
    public class PublicationContext
    {
        public PublicationContext(string content, DateTimeOffset createdOn)
        {
            Content = content;
            CreatedOn = createdOn;
        }

        public string Content { get; }

        public DateTimeOffset CreatedOn { get; }
    }
}