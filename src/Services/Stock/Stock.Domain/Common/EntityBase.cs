using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Stock.Domain.Common
{
    public class EntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
