using MongoDB.Driver;
using Stock.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Application.Contracts.Infrastructure
{
    public interface IStockContext
    {
        IMongoCollection<StockItem> StockItems { get; }
    }

}
