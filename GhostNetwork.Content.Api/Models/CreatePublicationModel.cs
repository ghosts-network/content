using System;
using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Content.Api.Models
{
    public class CreatePublicationModel
    {
        [Required]
        public string Content { get; set; }

        [Obsolete]
        public string AuthorId { get; set; }

        public UserInfoModel Author { get; set; }
    }
}