using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Content.MediaContent;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GhostNetwork.Content.MongoDb;

public class MediaStorage : IMediaStorage
{
    private readonly MongoDbContext context;

    public MediaStorage(MongoDbContext context)
    {
        this.context = context;
    }

    public async Task<Media> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var oId))
        {
            return null;
        }

        var filter = Builders<MediaEntity>.Filter.Eq(p => p.Id, oId);

        var entity = await context.Media
            .Find(filter)
            .FirstOrDefaultAsync();

        return entity == null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyCollection<Media>> SearchByKeyAsync(string key)
    {
        var filter = Builders<MediaEntity>.Filter.Eq(p => p.Key, key);

        var entities = await context.Media
            .Find(filter)
            .ToListAsync();

        return entities.Select(ToDomain).ToList();
    }

    public async Task<Dictionary<string, GroupedMedia>> FindGroupedMediaAsync(IReadOnlyCollection<string> keys)
    {
        var filter = Builders<MediaEntity>.Filter.In(x => x.Key, keys);

        var group = new BsonDocument()
        {
            {
                "_id", "$key"
            },
            {
                "media", new BsonDocument
                {
                    { "$push", "$$ROOT" }
                }
            },
            { "count", new BsonDocument("$sum", 1) }
        };

        var listMedia = await context.Media
            .Aggregate()
            .Match(filter)
            .Group<GroupedMediaEntity>(group)
            .ToListAsync();

        var dict = listMedia
            .ToDictionary(
                r => r.Id,
                r => new GroupedMedia(
                    r.Media.Select(ToDomain),
                    r.TotalCount));

        return keys
            .ToDictionary(
                key => key,
                key => dict.ContainsKey(key)
                    ? dict[key]
                    : new GroupedMedia(
                        Enumerable.Empty<Media>(),
                        0));
    }

    public async Task<string> InsertAsync(Media media)
    {
        var entity = new MediaEntity()
        {
            Id = default,
            Link = media.Link,
            Key = media.Key
        };

        await context.Media.InsertOneAsync(entity);

        return entity.Id.ToString();
    }

    public async Task DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var oId))
        {
            return;
        }

        var filter = Builders<MediaEntity>.Filter.Eq(x => x.Id, oId);

        await context.Media.DeleteOneAsync(filter);
    }

    public async Task DeleteByKeyAsync(string key)
    {
        var filter = Builders<MediaEntity>.Filter.Eq(p => p.Key, key);

        await context.Media.DeleteManyAsync(filter);
    }

    private static Media ToDomain(MediaEntity entity)
    {
        return new Media(entity.Id.ToString(), entity.Link, entity.Key);
    }
}