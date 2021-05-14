using AutoMapper;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Stock.Application.Contracts.Persistence;
using Stock.Grpc.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.Grpc.Services
{
    public class StockService : StockProtoService.StockProtoServiceBase
    {
        private readonly IStockRepository _repository;
        private readonly ILogger<StockService> _logger;
        private readonly IMapper _mapper;

        public StockService(IStockRepository repository, ILogger<StockService> logger, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }



        // Overriding rpc APIs (methods) defined in proto file
        public override async Task<StockModel> GetStock(GetStockRequest request, ServerCallContext context)
        {
            var stock = await _repository.GetStockByProduct(request.Id);

            if (stock == null)
            {
                _logger.LogError($"Stock with the product Id {request.Id} is not found.");

                throw new RpcException(new Status(StatusCode.NotFound, $"Stock with the product Id {request.Id} is not found."));
            }

            // We have to convert Stock model to Grpc StockModel.
            _logger.LogInformation("Stock is retrieved for ProductName: {productName}, Amount: {amount}", stock.ProductName, stock.Quantity);
            var stockModel = _mapper.Map<StockModel>(stock);
            return stockModel;
        }
    }
}
