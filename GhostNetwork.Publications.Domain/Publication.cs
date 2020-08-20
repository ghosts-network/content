namespace GhostNetwork.Publications.Domain
{
    public class Publication
    {
        public string Id { get; }
        public string Content { get; }

        public Publication(string id, string content)
        {
            Id = id;
            Content = content;
        }
    }
}