using System;
using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Content.Api.Models
{
    public class CreatePublicationModel
    {
        [Required]
        public string Content { get; set; }

        public string AuthorId { get; set; }

        [Obsolete]
        public UserInfoModel Author { get; set; }
    }
}