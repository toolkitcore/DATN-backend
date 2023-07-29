using HUST.Core.Constants;
using HUST.Core.Extensions;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Interfaces.Service;
using HUST.Core.Interfaces.Util;
using HUST.Core.Services;
using HUST.Core.Settings;
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

            
            // Cache sql
            services.AddTransient<ICacheSqlUtil, CacheSqlUtil>();

            // Session service
            services.AddSingleton<ISessionService, SessionService>();

            // Đăng nhập bằng jwt
            services.AddJwtAuthorization(configuration);
            
            // Inject service mapper, auth, cache
            services.UseAutoMapper();
            services.AddHttpContextAccessor();
            services.AddSingleton<IConfigUtil, ConfigUtil>();
            services.AddTransient<IAuthUtil, AuthUtil>();
            services.AddTransient<IHustServiceCollection, HustServiceCollection>();
        }
    }
}
