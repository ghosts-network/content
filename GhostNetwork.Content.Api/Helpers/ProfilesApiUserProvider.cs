using System;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Profiles.Api;
using GhostNetwork.Profiles.Client;

namespace GhostNetwork.Content.Api.Helpers
{
    public class ProfilesApiUserProvider : IUserProvider
    {
        private readonly IProfilesApi profilesApi;

        public ProfilesApiUserProvider(IProfilesApi profilesApi)
        {
            this.profilesApi = profilesApi;
        }

        public async Task<UserInfo> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                return null;
            }

            try
            {
                var result = await profilesApi.GetByIdAsync(guid);
                return result == null
                    ? null
                    : new UserInfo(result.Id, $"{result.FirstName} {result.LastName}", null);
            }
            catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}