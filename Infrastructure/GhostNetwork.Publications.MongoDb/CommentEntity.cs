using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Publications.MongoDb
{
    public class CommentEntity
    {
        public ObjectId Id { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("publicationId")]
        public string PublicationId { get; set; }

        [BsonElement("createOn")]
        public long CreateOn { get; set; }

        [BsonElement("replyId")]
        public string ReplyCommentId { get; set; }
    }
}
