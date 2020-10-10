using System;
using System.Collections.Generic;

namespace GhostNetwork.Publications
{
    public class Publication
    {
        public Publication(string id, string content, DateTimeOffset createdOn, IEnumerable<string> tags, DateTimeOffset updatedOn)
        {
            Id = id;
            Content = content;
            CreatedOn = createdOn;
            Tags = tags;
            UpdatedOn = updatedOn;
        }

        public string Id { get; }

        public string Content { get; }

        public DateTimeOffset CreatedOn { get; }

        public IEnumerable<string> Tags { get; }

        public DateTimeOffset UpdatedOn { get; }

        public bool IsUpdated => CreatedOn != UpdatedOn;
    }
}