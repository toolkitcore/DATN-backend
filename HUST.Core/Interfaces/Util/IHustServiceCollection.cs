using AutoMapper;
using HUST.Core.Interfaces.InfrastructureService;
using HUST.Core.Models.ServerObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Util
{
    public interface IHustServiceCollection
    {
        public IConfigUtil ConfigUtil { get; set; }
        public IAuthUtil AuthUtil { get; set; }
        public IDistributedCacheService CacheUtil { get; set; }
        public IMapper Mapper { get; set; }
        public ILogService LogUtil { get; set; }

        public void HandleControllerException(IServiceResult serviceResult, Exception ex);
    }
}
