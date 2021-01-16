using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Publications.MongoDb
{
    public class FeaturedInfoEntity
    {
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("comments")]
        public IEnumerable<CommentEntity> Comments { get; set; }

        [BsonElement("count")]
        public int TotalCount { get; set; }
    }
}
