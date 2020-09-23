using System;
using System.Collections.Generic;

namespace GhostNetwork.Publications.Domain
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

<<<<<<< HEAD
        public bool IsUpdated => CreatedOn != UpdatedOn;
=======
        public bool IsUpdated => CreatedOn.ToUnixTimeMilliseconds() != UpdatedOn.ToUnixTimeMilliseconds();
>>>>>>> f175740eea376e059d2d59d6d51bf8030f6fddc7
    }
}