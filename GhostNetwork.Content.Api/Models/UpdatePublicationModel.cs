using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Content.Api.Models
{
    public class UpdatePublicationModel
    {
        [Required]
        public string Content { get; set; }

        public IEnumerable<Media> Media { get; set; }
    }
}
