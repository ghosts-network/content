using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Content.Api.Models
{
    public class FeaturedQuery
    {
        [Required]
        public string[] Keys { get; set; }
    }
}
