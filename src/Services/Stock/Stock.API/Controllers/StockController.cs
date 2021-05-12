using Microsoft.AspNetCore.Mvc;
using Stock.Application.Contracts.Persistence;
using Stock.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _stockRepository;

        public StockController(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        [HttpPost]
        public async Task<ActionResult<StockItem>> CreateProduct([FromBody] StockItem stockItem)
        {
            stockItem.Id = Guid.NewGuid().ToString();
            await _stockRepository.Add(stockItem);
            return Ok();
        }

        [Route("[action]/{productName}", Name = "GetStockByProduct")]
        [HttpGet]
        public async Task<ActionResult<StockItem>> GetStockByProduct(string productName)
        {
            var stock = await _stockRepository.GetStockByProduct(productName);
            return Ok(stock);
        }
    }
}
