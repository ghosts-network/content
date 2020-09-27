using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Publications.Api.Models
{
    public class CreateCommentModel
    {
        [Required]
        public string PublicatioId { get; set; }

        [Required]
        public string Content { get; set; }

        public string ReplyCommentId { get; set; } 
    }
}
