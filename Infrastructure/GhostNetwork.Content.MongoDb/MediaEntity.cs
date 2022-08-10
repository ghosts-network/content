using System;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Content.MongoDb;

public class MediaEntity
{
    [BsonElement("link")]
    public string Link { get; init; }

    public static explicit operator Media(MediaEntity entity)
    {
        return entity == null
            ? null
            : new Media(entity.Link);
    }

    public static explicit operator MediaEntity(Media media)
    {
        return media == null
            ? null
            : new MediaEntity()
            {
                Link = media.Link
            };
    }
}