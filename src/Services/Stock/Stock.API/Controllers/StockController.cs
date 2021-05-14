using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stock.API.Model;
using Stock.Application.Contracts.Persistence;
using Stock.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Stock.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _stockRepository;
        private readonly ILogger<StockController> _logger;

        public StockController(IStockRepository stockRepository, ILogger<StockController> logger)
        {
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Route("[action]", Name = "GetAll")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StockItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<StockItem>>> GetAll()
        {
            var stocks = await _stockRepository.GetAll();
            return Success(stocks);
        }

        [HttpPost]
        public async Task<ActionResult<StockItem>> CreateProduct([FromBody] StockItem stockItem)
        {
            stockItem.Id = Guid.NewGuid().ToString();
            return Success(await _stockRepository.Add(stockItem));
        }

        [Route("[action]/{productId}", Name = "GetStockByProduct")]
        [HttpGet]
        public async Task<ActionResult<StockItem>> GetStockByProduct(string productId)
        {
            var stock = await _stockRepository.GetStockByProduct(productId);
            if (stock == null)
                return StockNotFound(productId);

            return Success(stock);
        }


        #region Private Methods
        private ActionResult Success(object data)
        {
            return new JsonResult(new ResponseModel
            {
                Success = true,
                Data = data
            });
        }


        private ActionResult StockNotFound(string productId)
        {
            var message = $"Stock not found for product {productId}.";
            _logger.LogWarning(message);
            return new JsonResult(new ResponseModel
            {
                Success = false,
                ErrorMessage = message
            });
        }
        #endregion
    }
}
