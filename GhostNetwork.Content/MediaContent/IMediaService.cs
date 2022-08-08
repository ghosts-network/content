using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace GhostNetwork.Content.MediaContent;

public interface IMediaService
{
    public Task<Media> GetByIdAsync(string id);

    public Task<IReadOnlyCollection<Media>> SearchByKeyAsync(string key);

    public Task<Dictionary<string, GroupedMedia>> FindGroupedMediaAsync(IReadOnlyCollection<string> keys);

    public Task<(DomainResult, string)> InsertAsync(string link, string key);

    public Task DeleteAsync(string id);

    public Task DeleteByKeyAsync(string key);
}

public class MediaService : IMediaService
{
    private readonly IMediaStorage mediaStorage;

    public MediaService(IMediaStorage mediaStorage)
    {
        this.mediaStorage = mediaStorage;
    }

    public async Task<Media> GetByIdAsync(string id)
    {
        return await mediaStorage.GetByIdAsync(id);
    }

    public async Task<IReadOnlyCollection<Media>> SearchByKeyAsync(string key)
    {
        return await mediaStorage.SearchByKeyAsync(key);
    }

    public async Task<Dictionary<string, GroupedMedia>> FindGroupedMediaAsync(IReadOnlyCollection<string> keys)
    {
        return await mediaStorage.FindGroupedMediaAsync(keys);
    }

    public async Task<(DomainResult, string)> InsertAsync(string link, string key)
    {
        var media = Media.New(link, key);

        var id = await mediaStorage.InsertAsync(media);

        return (DomainResult.Success(), id);
    }

    public async Task DeleteAsync(string id)
    {
        await mediaStorage.DeleteAsync(id);
    }

    public async Task DeleteByKeyAsync(string key)
    {
        await mediaStorage.DeleteByKeyAsync(key);
    }
}