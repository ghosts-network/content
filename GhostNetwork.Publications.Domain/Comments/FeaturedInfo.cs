using System.Collections.Generic;

namespace GhostNetwork.Publications.Comments
{
    public class FeaturedInfo
    {
        public IEnumerable<Comment> Comments { get; set; }

        public int TotalCount { get; set; }
    }
}
