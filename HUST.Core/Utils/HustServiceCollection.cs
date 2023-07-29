using AutoMapper;
using HUST.Core.Interfaces.InfrastructureService;
using HUST.Core.Interfaces.Util;
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
        public IDistributedCacheService CacheUtil { get; set; }
        public IMapper Mapper { get; set; }

        public ILogService LogUtil { get; set; }


        public HustServiceCollection(IConfigUtil configUtil, IAuthUtil authUtil, IDistributedCacheService cacheUtil, IMapper mapper, ILogService logUtil)
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
