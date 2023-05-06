using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Utils
{
    public class HustRedisCache : RedisCache, IHustDistributedCache
    {
        public HustRedisCache(IOptions<HustRedisCacheOptions> optionsAccessor) : base(optionsAccessor)
        {
        }
    }

    public class HustRedisCacheOptions : RedisCacheOptions
    {

    }
}
