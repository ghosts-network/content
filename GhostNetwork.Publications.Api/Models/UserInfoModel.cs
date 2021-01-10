using System;
using System.ComponentModel.DataAnnotations;

namespace GhostNetwork.Publications.Api.Models
{
    public class UserInfoModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string FullName { get; set; }

        public string AvatarUrl { get; set; }

        public static explicit operator UserInfo(UserInfoModel model)
        {
            return model == null
                ? null
                : new UserInfo(model.Id, model.FullName, model.AvatarUrl);
        }
    }
}