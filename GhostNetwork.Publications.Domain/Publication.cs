using System;
using System.Collections.Generic;

namespace GhostNetwork.Publications.Domain
{
    public class Publication
    {
        public Publication(string id, string content, DateTimeOffset createdOn, IEnumerable<string> tags)
        {
            Id = id;
            Content = content;
            CreatedOn = createdOn;
            Tags = tags;
        }

        public string Id { get; }

        public string Content { get; }

        public DateTimeOffset CreatedOn { get; }

        public IEnumerable<string> Tags { get; }
    }
}