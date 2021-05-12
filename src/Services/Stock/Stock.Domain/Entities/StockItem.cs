using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Stock.Domain.Common;

namespace Stock.Domain.Entities
{
    public class StockItem : EntityBase
    {
        [BsonElement("ProductName")]
        public string ProductName { get; set; }

        public int Quantity { get; set; }
    }
}
