namespace GhostNetwork.Publications.Comments
{
    public class CommentContext
    {
        public CommentContext(string content)
        {
            Content = content;
        }

        public string Content { get; set; }
    }
}
