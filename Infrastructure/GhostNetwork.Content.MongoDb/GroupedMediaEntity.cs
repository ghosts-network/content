using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Content.MongoDb;

public class GroupedMediaEntity
{
    [BsonElement("_id")]
    public string Id { get; set; }

    [BsonElement("media")]
    public IEnumerable<MediaEntity> Media { get; set; }

    [BsonElement("count")]
    public int TotalCount { get; set; }
}