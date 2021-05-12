using MongoDB.Driver;
using Stock.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Infrastructure.Persistence
{
    public class StockContextSeed
    {
        public static void SeedData(IMongoCollection<StockItem> stockItemCollection)
        {
            bool existStock = stockItemCollection.Find(p => true).Any();

            if (!existStock)
            {
                stockItemCollection.InsertManyAsync(GetPreconfiguredStockItems());
            }
        }

        private static IEnumerable<StockItem> GetPreconfiguredStockItems()
        {
            return new List<StockItem>()
            {
                new StockItem()
                {
                    Id = "602d2149e773f2a3990b47f5",
                    ProductName = "Iphone X",
                    Quantity = 500
                },
                 new StockItem()
                {
                    Id = "602d2149e773f2a3990b47f6",
                    ProductName = "Samsung S20",
                    Quantity = 200
                },
                  new StockItem()
                {
                    Id = "602d2149e773f2a3990b47f7",
                    ProductName = "Huawei P40",
                    Quantity = 70
                },
                   new StockItem()
                {
                    Id = "602d2149e773f2a3990b47f8",
                    ProductName = "Xiaomi Redmi Note 9",
                    Quantity = 160
                },

            };
        }
    }
}
