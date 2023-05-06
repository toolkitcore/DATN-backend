using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Utils
{
    public interface IConfigUtil
    {
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Lấy thông tin cấu hình app setting
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetAppSetting(string key, string defaultValue = null);

        /// <summary>
        /// Lấy thông tin API Url đã được config
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetAPIUrl(string key);

        /// <summary>
        /// Lấy thông tin chuỗi connection string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetConnectionString(string name);
    }
}
