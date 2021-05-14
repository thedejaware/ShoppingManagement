using Stock.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Application.Contracts.Persistence
{
    public interface IStockRepository
    {
        Task<IEnumerable<StockItem>> GetAll();
        Task<StockItem> GetStockByProduct(string productId);
        Task<StockItem> Add(StockItem stockItem);
        Task<bool> Update(StockItem stockItem);
        Task<bool> Delete(string id);

    }
}
