using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HUST.Api.Controllers
{
    public class AuditLogController : BaseApiController
    {
        #region Fields
        private readonly IAuditLogService _service;
        #endregion

        #region Constructor
        public AuditLogController(IHustServiceCollection serviceCollection, IAuditLogService service) : base(serviceCollection)
        {
            _service = service;
        }
        #endregion

        /// <summary>
        /// Lấy lịch sử truy cập của người dùng
        /// </summary>
        /// <param name="searchFilter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="dateForm"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        [HttpGet("get_logs")]
        public async Task<IServiceResult> GetLogs([FromQuery]string searchFilter, 
            int? pageIndex, int? pageSize, string dateFrom, string dateTo)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.GetLogs(searchFilter, pageIndex, pageSize, dateFrom, dateTo);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Lưu lịch sử truy cập của người dùng
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("save_log")]
        public async Task<IServiceResult> SaveLog([FromBody] AuditLog param)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.SaveLog(param);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }
    }
}
