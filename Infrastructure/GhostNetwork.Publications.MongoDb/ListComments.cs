using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Publications.MongoDb
{
    public class ListComments
    {
        [BsonElement("_id")]
        public string Id { get; set; }

        public IEnumerable<CommentEntity> Comments { get; set; }
    }
}
