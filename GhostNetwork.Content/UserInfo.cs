using System;

namespace GhostNetwork.Content
{
    public class UserInfo
    {
        public UserInfo(Guid id, string fullName, string avatarUrl)
        {
            Id = id;
            FullName = fullName;
            AvatarUrl = avatarUrl;
        }

        public Guid Id { get; }

        public string FullName { get; }

        public string AvatarUrl { get; }
    }
}