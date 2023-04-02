using HUST.Core.Constants;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Utils
{
    public class ConfigUtil : IConfigUtil
    {
        #region Declaration

        private readonly IConfigurationSection _appSettings;
        private readonly IConfigurationSection _connectionStrings;
        private readonly IConfigurationSection _apiURLs;

        #endregion

        #region Properties

        public IConfiguration Configuration { get; private set; }

        #endregion

        #region Constructor

        public ConfigUtil(IConfiguration configuration)
        {
            Configuration = configuration;
            _appSettings = configuration.GetSection(AppSettingKey.AppSettingsSection);
            _connectionStrings = configuration.GetSection(AppSettingKey.ConnectionStringsSection);
            _apiURLs = configuration.GetSection(AppSettingKey.APIUrlSection);

        }

        #endregion

        #region Methods

        /// <summary>
        /// Lấy thông tin cấu hình app setting
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetAppSetting(string key, string defaultValue = null)
        {
            var value = _appSettings[key];
            if (string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(defaultValue))
            {
                value = defaultValue;
            }

            return value;
        }

        /// <summary>
        /// Lấy thông tin API Url đã được config
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetAPIUrl(string key)
        {
            return _apiURLs[key];
        }

        /// <summary>
        /// Lấy thông tin chuỗi connection string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetConnectionString(string name)
        {
            return _connectionStrings[name];
        }

        #endregion
    }
}
