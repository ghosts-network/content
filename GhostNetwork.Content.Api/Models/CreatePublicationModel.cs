using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Content.Api.Models
{
    public class CreatePublicationModel
    {
        [Required]
        public string Content { get; set; }

        [Obsolete]
        public string AuthorId { get; set; }

        public IEnumerable<Media> Media { get; set; }

        public UserInfo Author { get; set; }
    }
}