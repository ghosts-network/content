using System;
using System.Collections.Generic;
using System.Text;

namespace GhostNetwork.Publications.Domain
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
