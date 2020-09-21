using System;
using System.Collections.Generic;

namespace GhostNetwork.Publications.Domain
{
    public class Publication
    {
        public Publication(string id, string content, DateTimeOffset createdOn, IEnumerable<string> tags, DateTimeOffset updatedOn, bool isUpdated)
        {
            Id = id;
            Content = content;
            CreatedOn = createdOn;
            Tags = tags;
            UpdatedOn = updatedOn;
            IsUpdated = isUpdated;
        }

        public string Id { get; }

        public string Content { get; }

        public DateTimeOffset CreatedOn { get; }

        public IEnumerable<string> Tags { get; }

        public DateTimeOffset UpdatedOn { get; }

        public bool IsUpdated { get; }
    }
}