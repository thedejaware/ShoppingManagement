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
        private readonly IStockDatabaseSettings _databaseSettings;
        public StockContext(IStockDatabaseSettings databaseSettings)
        {
            //_databaseSettings = databaseSettings.Value;
            _databaseSettings = databaseSettings;

            // var client2 = new MongoClient(_databaseSettings.ConnectionString);
            //var database = client.GetDatabase(_databaseSettings.DatabaseName);

            //StockItems = database.GetCollection<StockItem>(_databaseSettings.CollectionName);

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("StockDb");

            StockItems = database.GetCollection<StockItem>("StockItems");
            StockContextSeed.SeedData(StockItems);
        }
        public IMongoCollection<StockItem> StockItems { get; }
    }
}
