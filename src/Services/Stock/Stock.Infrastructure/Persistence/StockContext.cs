using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Stock.Application.Contracts.Infrastructure;
using Stock.Application.Models;
using Stock.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Infrastructure.Persistence
{
    public class StockContext : IStockContext
    {
        //public DatabaseSettings _databaseSettings { get; }
        private readonly DatabaseSettings _databaseSettings;
        public StockContext(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseSettings = databaseSettings.Value;

            var client = new MongoClient(_databaseSettings.ConnectionString);
            
            var database = client.GetDatabase(_databaseSettings.DatabaseName);

            StockItems = database.GetCollection<StockItem>(_databaseSettings.CollectionName);

            StockContextSeed.SeedData(StockItems);
        }
        
        public IMongoCollection<StockItem> StockItems { get; }
    }
}
