using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DeliveryProjectAzure.Services
{
    public class ServiceCart
    {
        private readonly IDatabase redisCache;

        public ServiceCart(IConnectionMultiplexer connectionMultiplexer)
        {
            redisCache = connectionMultiplexer.GetDatabase();
        }

        public async Task<List<int>> GetCartAsync(string cacheKey)
        {
            RedisValue cartValue = await redisCache.StringGetAsync(cacheKey);

            if (cartValue.HasValue)
            {
                return JsonConvert.DeserializeObject<List<int>>(cartValue);
            }

            return null;
        }

        public async Task SetCartAsync(string cacheKey, List<int> cart)
        {
            string cartValue = JsonConvert.SerializeObject(cart);
            await redisCache.StringSetAsync(cacheKey, cartValue);
        }

        public async Task RemoveCartAsync(string cacheKey)
        {
            await redisCache.KeyDeleteAsync(cacheKey);
        }
    }
}
