using System;
using System.Collections.Generic;

namespace GhostNetwork.Content.Publications
{
    public class Publication
    {
        public Publication(string id, string content, IEnumerable<string> tags, UserInfo author, DateTimeOffset createdOn, DateTimeOffset updatedOn, IEnumerable<Media> media)
        {
            Id = id;
            Content = content;
            Tags = tags;
            Author = author;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
            Media = media;
        }

        public string Id { get; }

        public string Content { get; private set; }

        public IEnumerable<string> Tags { get; private set; }

        public UserInfo Author { get; }

        public DateTimeOffset CreatedOn { get; }

        public DateTimeOffset UpdatedOn { get; private set; }

        public bool IsUpdated => CreatedOn != UpdatedOn;

        public IEnumerable<Media> Media { get; private set; }

        public static Publication New(string content, UserInfo author, Func<string, IEnumerable<string>> tagsFetcher, IEnumerable<Media> media)
        {
            var now = DateTimeOffset.UtcNow;

            return new Publication(default, content, tagsFetcher(content), author, now, now, media);
        }

        public Publication Update(string content, Func<string, IEnumerable<string>> tagsFetcher, IEnumerable<Media> media)
        {
            Content = content;
            UpdatedOn = DateTimeOffset.UtcNow;
            Tags = tagsFetcher(content);
            Media = media;

            return this;
        }
    }
}