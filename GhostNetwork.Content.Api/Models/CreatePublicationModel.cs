using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Content.Api.Models
{
    public class CreatePublicationModel
    {
        [Required]
        public string Content { get; set; }

        [Obsolete("Use Author instead")]
        public string AuthorId { get; set; }

        public UserInfo Author { get; set; }

        public IEnumerable<CreateMediaModel> Media { get; set; } = new List<CreateMediaModel>();
    }
}