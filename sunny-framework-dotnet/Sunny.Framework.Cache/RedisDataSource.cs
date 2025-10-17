using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Sunny.Framework.Cache
{
    public class RedisDataSource
    {
        private readonly Dictionary<string, IConnectionMultiplexer> _instances = new();

        public RedisDataSource(IConfiguration config)
        {
            var redisProperties = config.GetSection("Data:Redis").Get<RedisProperties>();
            _instances["Default"] = ConnectionMultiplexer.Connect(redisProperties.Url);
            foreach (var t in redisProperties.Instances??new Dictionary<string, RedisOption>())
            {
                _instances[t.Key] = ConnectionMultiplexer.Connect(t.Value.Url);
            }
        }

        public IConnectionMultiplexer GetDefault()
        {
            return _instances["Default"];
        }

        public IConnectionMultiplexer GetInstance(string key)
        {
            return _instances[key];
        }
    }
}