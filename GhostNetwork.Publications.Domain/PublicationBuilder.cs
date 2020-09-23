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
<<<<<<< HEAD
            return new Publication(string.Empty, content, dateNow, tagsFetcher.Fetch(content), dateNow);
=======
            return new Publication(string.Empty, content, DateTimeOffset.Now, tagsFetcher.Fetch(content), DateTimeOffset.Now);
>>>>>>> f175740eea376e059d2d59d6d51bf8030f6fddc7
        }
    }
}