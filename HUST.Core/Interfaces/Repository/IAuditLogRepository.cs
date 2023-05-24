using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using HUST.Core.Models.ServerObject;
using System;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Repository
{

    public interface IAuditLogRepository: IBaseRepository<audit_log>
    {
        /// <summary>
        /// Lấy danh sách lịch sử truy cập (lọc và phân trang)
        /// </summary>
        /// <param name="searchFilter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        Task<FilterResult<AuditLog>> GetLogsByFilter(string userId, string searchFilter, 
            int? pageIndex, int? pageSize, DateTime? dateFrom, DateTime? dateTo);
    }
}
