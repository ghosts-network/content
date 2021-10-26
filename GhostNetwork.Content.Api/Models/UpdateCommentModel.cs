using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Content.Api.Models
{
    public class UpdateCommentModel
    {
        [Required]
        public string Content { get; set; }
    }
}