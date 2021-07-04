using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Content.MongoDb
{
    public class CommentEntity
    {
        public ObjectId Id { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("key")]
        public string Key { get; set; }

        [BsonElement("createOn")]
        public long CreateOn { get; set; }

        [BsonElement("replyId")]
        public string ReplyCommentId { get; set; }

        [BsonElement("author")]
        public UserInfoEntity Author { get; set; }
    }
}
