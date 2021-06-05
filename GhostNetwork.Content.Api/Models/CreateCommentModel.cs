using System;
using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Content.Api.Models
{
    public class CreateCommentModel
    {
        [Obsolete]
        public string PublicationId { get; set; }
        
        public string Key { get; set; }

        [Required]
        public string Content { get; set; }

        public string ReplyCommentId { get; set; }

        public string AuthorId { get; set; }
    }
}
