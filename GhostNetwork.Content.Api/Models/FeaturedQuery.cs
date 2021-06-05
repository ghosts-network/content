using System;

namespace GhostNetwork.Content.Api.Models
{
    public class FeaturedQuery
    {
        [Obsolete]
        public string[] PublicationIds { get; set; }

        public string[] Keys { get; set; }
    }
}
