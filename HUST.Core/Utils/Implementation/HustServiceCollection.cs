using AutoMapper;
using HUST.Core.Models.ServerObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Utils
{
    public class HustServiceCollection : IHustServiceCollection
    {
        public IConfigUtil ConfigUtil { get; set; }
        public IAuthUtil AuthUtil { get; set; }
        public IDistributedCacheUtil CacheUtil { get; set; }
        public IMapper Mapper { get; set; }

        public ILogUtil LogUtil { get; set; }


        public HustServiceCollection(IConfigUtil configUtil, IAuthUtil authUtil, IDistributedCacheUtil cacheUtil, IMapper mapper, ILogUtil logUtil)
        {
            ConfigUtil = configUtil;
            AuthUtil = authUtil;
            CacheUtil = cacheUtil;
            Mapper = mapper;
            LogUtil = logUtil;
        }

        /// <summary>
        /// Xử lý ngoại lệ ở controller
        /// </summary>
        /// <param name="serviceResult"></param>
        /// <param name="ex"></param>
        public void HandleControllerException(IServiceResult serviceResult, Exception ex)
        {
            serviceResult.OnException(ex);
            LogUtil.LogError(ex, ex.Message);
        }
    }
}
