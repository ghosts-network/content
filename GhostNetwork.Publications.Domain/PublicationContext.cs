using System;

namespace GhostNetwork.Publications.Domain
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