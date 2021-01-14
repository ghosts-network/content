using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Publications.Api.Models
{
    public class FeaturedQuery
    {
        [Required]
        public string[] PublicationIds { get; set; }
    }
}
