using Grpc.Core;
using Stock.Grpc.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.GrpcServices
{
    public class StockGrpcService
    {
        private readonly StockProtoService.StockProtoServiceClient _client;

        public StockGrpcService(StockProtoService.StockProtoServiceClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<StockModel> GetStock(string productId)
        {
            try
            {
                var request = new GetStockRequest { Id = productId };
                return await _client.GetStockAsync(request);
            }
            catch (RpcException ex)
            {
                return null;
            }
            
        }
    }
}
