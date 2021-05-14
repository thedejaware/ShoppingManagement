using Basket.Application.Contracts.Persistence;
using Basket.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Basket.Infrastructure.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache;
        }

        public async Task<ShoppingCart> GetBasket(string userName)
        {
            var basket = await _redisCache.GetAsync(userName);
            if (basket == null)
                return null;

            var serializedBasket = Encoding.UTF8.GetString(basket);
            return JsonConvert.DeserializeObject<ShoppingCart>(serializedBasket);
        }

        public async Task DeleteBasket(string userName)
        {
            await _redisCache.RemoveAsync(userName);
        }


        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await _redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));

            return await GetBasket(basket.UserName);
        }

        public async Task<bool> GetItemInBasket(string productId, string username)
        {
            var basket = await _redisCache.GetAsync(username);

            var serializedBasket = Encoding.UTF8.GetString(basket);

            var shoppingCart = JsonConvert.DeserializeObject<ShoppingCart>(serializedBasket);

            var itemFound = shoppingCart.Items.FirstOrDefault(p => p.ProductId == productId);

            if (itemFound == null)
                return false;

            return true;
        }

        public async Task<ShoppingCart> CreateBasket(ShoppingCart basket)
        {
            await _redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));

            return await GetBasket(basket.UserName);
        }
    }
}
