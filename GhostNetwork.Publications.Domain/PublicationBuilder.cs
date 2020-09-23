using System;

namespace GhostNetwork.Publications.Domain
{
    public class PublicationBuilder
    {
        private readonly IHashTagsFetcher tagsFetcher;
        private readonly DateTimeOffset dateNow = DateTimeOffset.Now;

        public PublicationBuilder(IHashTagsFetcher tagsFetcher)
        {
            this.tagsFetcher = tagsFetcher;
        }

        public Publication Build(string content)
        {
            return new Publication(string.Empty, content, dateNow, tagsFetcher.Fetch(content), dateNow);
        }
    }
}