using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Content.MongoDb;

public class MediaEntity
{
    public ObjectId Id { get; init; }

    [BsonElement("link")]
    public string Link { get; init; }

    [BsonElement("key")]
    public string Key { get; init; }
}