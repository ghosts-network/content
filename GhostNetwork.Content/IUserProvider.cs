using System.Threading.Tasks;

namespace GhostNetwork.Content
{
    public interface IUserProvider
    {
        Task<UserInfo> GetByIdAsync(string id);
    }
}