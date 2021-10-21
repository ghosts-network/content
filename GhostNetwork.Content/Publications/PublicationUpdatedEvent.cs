namespace GhostNetwork.Content.Publications
{
    public class PublicationUpdatedEvent : Event
    {
        public PublicationUpdatedEvent(string id, string content, UserInfo author)
        {
            Id = id;
            Content = content;
            Author = author;
        }

        public string Id { get; }
        public string Content { get; }
        public UserInfo Author { get; }
    }
}