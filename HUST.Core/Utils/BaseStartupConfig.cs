using HUST.Core.Constants;
using HUST.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Utils
{
    /// <summary>
    /// Cấu hình chung cho startup
    /// </summary>
    public static class BaseStartupConfig
    {
        public static void ConfigureServices(ref IServiceCollection services, IConfiguration configuration)
        {
            // Config json
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DateFormatHandling = SerializeUtil.JSONDateFormatHandling;
                    options.SerializerSettings.DateFormatString = SerializeUtil.JSONDateFormatString;
                    options.SerializerSettings.DateTimeZoneHandling = SerializeUtil.JSONDateTimeZoneHandling;
                    options.SerializerSettings.NullValueHandling = SerializeUtil.JSONNullValueHandling;
                    options.SerializerSettings.ReferenceLoopHandling = SerializeUtil.JSONReferenceLoopHandling;
                    options.SerializerSettings.ContractResolver = null;
                });

            // Đăng nhập bằng jwt
            services.AddJwtAuthorization(configuration);
            //services.AddAuthorization();

            // Cache redis
            var redisCache = configuration.GetConnectionString(ConnectionStringSettingKey.RedisCache);
            if (!string.IsNullOrEmpty(redisCache))
            {
                //services.Configure<HustRedisCacheOptions>(config =>
                //{
                //    config.Configuration = redisCache;
                //    config.InstanceName = CacheKey.HustRedisCacheKey;
                //});
                services.AddStackExchangeRedisCache(option =>
                {
                    option.Configuration = redisCache;
                    option.InstanceName = CacheKey.HustRedisCacheKey;
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            

            // Inject service mapper, auth, cache
            services.UseAutoMapper();
            services.AddHttpContextAccessor();
            services.AddSingleton<IConfigUtil, ConfigUtil>();
            services.AddTransient<IAuthUtil, AuthUtil>();
            //services.AddTransient<IHustDistributedCache, HustRedisCache>();
            //services.AddTransient<DistributedCacheHelper>();
            services.AddTransient<IDistributedCacheUtil, DistributedCacheUtil>();
            services.AddTransient<IHustServiceCollection, HustServiceCollection>();
        }
    }
}
