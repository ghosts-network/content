using System;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Content.MongoDb;

public class MediaEntity
{
    [BsonId]
    public Guid Id { get; init; }

    [BsonElement("link")]
    public string Link { get; init; }

    public static explicit operator Media(MediaEntity entity)
    {
        return entity == null
            ? null
            : new Media(entity.Id, entity.Link);
    }

    public static explicit operator MediaEntity(Media media)
    {
        return media == null
            ? null
            : new MediaEntity()
            {
                Id = media.Id,
                Link = media.Link
            };
    }
}