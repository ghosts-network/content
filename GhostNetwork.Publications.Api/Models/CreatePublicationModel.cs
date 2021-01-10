using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Publications.Api.Models
{
    public class CreatePublicationModel
    {
        [Required]
        public string Content { get; set; }

        public UserInfoModel Author { get; set; }
    }
}