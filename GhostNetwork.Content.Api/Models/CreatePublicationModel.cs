using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Content.Api.Models
{
    public class CreatePublicationModel
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public UserInfoModel Author { get; set; }
    }
}