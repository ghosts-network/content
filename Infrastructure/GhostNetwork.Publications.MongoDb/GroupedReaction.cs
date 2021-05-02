using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Publications.MongoDb
{
    public class GroupedReaction
    {
        [BsonElement("_id")]
        public GroupedReactionsKey Key { get; set; }

        [BsonElement("count")]
        public int Count { get; set; }

        public class GroupedReactionsKey
        {
            [BsonElement("key")]
            public string Key { get; set; }

            [BsonElement("type")]
            public string Type { get; set; }
        }
    }
}