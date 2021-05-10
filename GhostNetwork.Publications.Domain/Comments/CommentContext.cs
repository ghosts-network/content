namespace GhostNetwork.Publications.Comments
{
    public class CommentContext
    {
        public CommentContext(string content, string replyId = null)
        {
            Content = content;
            ReplyId = replyId;
        }

        public string Content { get; }

        public string ReplyId { get; }
    }
}
