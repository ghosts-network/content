using System.Collections.Generic;

namespace GhostNetwork.Content.Comments
{
    public class FeaturedInfo
    {
        public FeaturedInfo(IEnumerable<Comment> comments, int totalCount)
        {
            Comments = comments;
            TotalCount = totalCount;
        }

        public IEnumerable<Comment> Comments { get; }

        public int TotalCount { get; }
    }
}
