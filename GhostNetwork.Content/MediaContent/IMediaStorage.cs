using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Content.MediaContent;

public interface IMediaStorage
{
    public Task<Media> GetByIdAsync(string id);

    public Task<IReadOnlyCollection<Media>> SearchByKeyAsync(string key);

    public Task<Dictionary<string, GroupedMedia>> FindGroupedMediaAsync(IReadOnlyCollection<string> keys);

    public Task<string> InsertAsync(Media media);

    public Task DeleteAsync(string id);

    public Task DeleteByKeyAsync(string key);
}