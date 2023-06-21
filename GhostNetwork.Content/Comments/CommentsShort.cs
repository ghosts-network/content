using System.Collections.Generic;

namespace GhostNetwork.Content.Comments
{
    public class CommentsShort
    {
        public CommentsShort(IEnumerable<Comment> topComments, long totalCount)
        {
            TopComments = topComments;
            TotalCount = totalCount;
        }

        public IEnumerable<Comment> TopComments { get; }

        public long TotalCount { get; }
    }
}
