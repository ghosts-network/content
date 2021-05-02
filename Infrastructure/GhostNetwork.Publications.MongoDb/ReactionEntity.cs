using MongoDB.Bson;

namespace GhostNetwork.Publications.MongoDb
{
    public class ReactionEntity
    {
        public ObjectId Id { get; set; }

        public string Key { get; set; }

        public string Author { get; set; }

        public string Type { get; set; }
    }
}