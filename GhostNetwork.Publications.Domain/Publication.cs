namespace GhostNetwork.Publications.Domain
{
    public class Publication
    {
        public Publication(string id, string content)
        {
            Id = id;
            Content = content;
        }

        public string Id { get; }

        public string Content { get; }
    }
}