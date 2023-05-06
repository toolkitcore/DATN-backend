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
        public const string SendgridMailSettingsSection = "SendgridMailSettings";
        public const string MailSettingsSection = "MailSettings";

        public const string JwtSecretKey = "JwtSecretKey";
        public const string JwtIssuer = "JwtIssuer";
        public const string JwtAudience = "JwtAudience";
        public const string JwtLifeTime = "JwtLifeTime";
        public const string SecretKey = "SecretKey";
    }

    public static class ConnectionStringSettingKey
    {
        public const string RedisCache = "RedisCache";
        public const string Database = "Database";
    }

    public static class SendgridMailSettingsKey
    {
        public const string SendGridKey = "SendGridKey";
        public const string FromAddress = "FromAddress";
        public const string FromName = "FromName";
    }

    /// <summary>
    /// Hằng key cache
    /// </summary>
    public static class CacheKey
    {
        public const string HustInstanceCache = "Hust";

        public const string SessionCacheKey = "session_{0}";
    }

}
