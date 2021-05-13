using MongoDB.Driver;
using Stock.Application.Contracts.Infrastructure;
using Stock.Application.Contracts.Persistence;
using Stock.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Infrastructure.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly IStockContext _context;

        public StockRepository(IStockContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<StockItem> GetStockByProduct(string productName)
        {
            return await _context
                             .StockItems
                             .Find(p => p.ProductName == productName)
                             .FirstOrDefaultAsync();
        }

        public async Task Add(StockItem stockItem)
        {
            await _context.StockItems.InsertOneAsync(stockItem);
        }

        public async Task<bool> Delete(string id)
        {
            FilterDefinition<StockItem> filter = Builders<StockItem>.Filter.Eq(p => p.Id, id);

            DeleteResult deletedResult = await _context
                                            .StockItems
                                            .DeleteOneAsync(filter);

            return deletedResult.IsAcknowledged
                        && deletedResult.DeletedCount > 0;
        }

        public async Task<bool> Update(StockItem stockItem)
        {
            var updatedResult = await _context
                                    .StockItems
                                    .ReplaceOneAsync(filter: g => g.Id == stockItem.Id, replacement: stockItem);

            return updatedResult.IsAcknowledged
                       && updatedResult.ModifiedCount > 0;
        }
    }
}
