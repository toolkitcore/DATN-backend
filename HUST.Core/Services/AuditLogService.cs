using HUST.Core.Constants;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.Entity;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using User = HUST.Core.Models.DTO.User;

namespace HUST.Core.Services
{
    /// <summary>
    /// Serivce xử lý log
    /// </summary>
    public class AuditLogService : BaseService, IAuditLogService
    {
        #region Field

        private readonly IAuditLogRepository _repository;
        private readonly IHttpContextAccessor _httpContext;
        #endregion

        #region Constructor

        public AuditLogService(
            IAuditLogRepository repository,
            IHttpContextAccessor httpContext,
            IHustServiceCollection serviceCollection) : base(serviceCollection)
        {
            _repository = repository;
            _httpContext = httpContext;
        }

        #endregion

        #region Method

        /// <summary>
        /// Lấy lịch sử truy cập của người dùng
        /// </summary>
        /// <param name="searchFilter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="dateForm"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public async Task<IServiceResult> GetLogs(string searchFilter, int? pageIndex, int? pageSize, string dateFrom, string dateTo)
        {
            var res = new ServiceResult();

            var userId = this.ServiceCollection.AuthUtil.GetCurrentUserId()?.ToString();
            pageIndex = pageIndex ?? 0;
            pageSize = pageSize ?? 0;

            var datetimeFrom = !string.IsNullOrEmpty(dateFrom) ? DateTime.Parse(dateFrom) : (DateTime?)null;
            var datetimeTo = !string.IsNullOrEmpty(dateTo) ? DateTime.Parse(dateTo) : (DateTime?)null;

            var data = await _repository.GetLogsByFilter(userId, searchFilter, 
                pageIndex, pageSize, datetimeFrom, datetimeTo);
            return res.OnSuccess(data);
        }

        /// <summary>
        /// Lưu lịch sử truy cập của người dùng
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<IServiceResult> SaveLog(Models.DTO.AuditLog param)
        {
            var res = new ServiceResult();

            param.UserId = this.ServiceCollection.AuthUtil.GetCurrentUserId();
            param.UserAgent = _httpContext.HttpContext.Request.Headers["User-Agent"].ToString();
            param.CreatedDate ??= DateTime.Now;

            var result = await _repository.Insert(this.ServiceCollection.Mapper.Map<audit_log>(param));
            if (!result)
            {
                return res.OnError(ErrorCode.Err9999);
            }

            return res.OnSuccess();
        }
        #endregion

    }


}
