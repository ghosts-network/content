using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Publications.Api.Models
{
    public class UpdatePublicationModel
    {
        [Required]
        public string Content { get; set; }
    }
}
