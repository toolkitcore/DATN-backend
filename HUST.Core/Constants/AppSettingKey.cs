using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Constants
{
    /// <summary>
    /// Key appsetting
    /// </summary>
    public static class AppSettingKey
    {
        public const string AppSettingsSection = "AppSettings";
        public const string ConnectionStringsSection = "ConnectionStrings";
        public const string APIUrlSection = "APIUrl";
        public const string JwtSecretKey = "JwtSecretKey";
        public const string JwtIssuer = "JwtIssuer";
        public const string JwtAudience = "JwtAudience";
        
        public class ConnectionStrings
        {
            public const string RedisCache = "ConnectionStrings:RedisCache";
            public const string Database = "ConnectionStrings:Database";
        }

    }

    public static class ConnectionStringSettingKey
    {
        public const string RedisCache = "RedisCache";
        public const string Database = "Database";
    }

}
