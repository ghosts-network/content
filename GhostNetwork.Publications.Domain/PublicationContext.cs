namespace GhostNetwork.Publications
{
    public class PublicationContext
    {
        public PublicationContext(string content)
        {
            Content = content;
        }

        public string Content { get; }
    }
}