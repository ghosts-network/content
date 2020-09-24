using System;

namespace GhostNetwork.Publications.Domain
{
    public class PublicationBuilder
    {
        private readonly IHashTagsFetcher tagsFetcher;
        private readonly DateTimeOffset timeNow;

        public PublicationBuilder(IHashTagsFetcher tagsFetcher)
        {
            this.tagsFetcher = tagsFetcher;
            timeNow = DateTimeOffset.Now;
        }

        public Publication Build(string content)
        {
            return new Publication(string.Empty, content, timeNow, tagsFetcher.Fetch(content), timeNow);
        }
    }
}