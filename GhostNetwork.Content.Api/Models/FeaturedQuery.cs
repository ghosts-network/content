using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Content.Api.Models
{
    public class FeaturedQuery
    {
        [Required]
        public string[] PublicationIds { get; set; }
    }
}
