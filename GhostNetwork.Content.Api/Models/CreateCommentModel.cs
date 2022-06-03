using System;
using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Content.Api.Models
{
    public class CreateCommentModel
    {
        public string Key { get; set; }

        [Required]
        public string Content { get; set; }

        public string ReplyCommentId { get; set; }

        [Obsolete]
        public string AuthorId { get; set; }

        public UserInfo Author { get; set; }
    }
}
