using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Publications.Api.Models
{
    public class FindCommentsByIdsModel
    {
        [Required]
        public string[] PublicationIds { get; set; }

        public int Take { get; set; }
    }
}
