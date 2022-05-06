using System;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Content.MongoDb;

public class MediaEntity
{
    [BsonId]
    public Guid Id { get; init; }

    [BsonElement("link")]
    public string Link { get; init; }
}