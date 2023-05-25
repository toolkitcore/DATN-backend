using Dapper;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace HUST.Infrastructure.Repositories
{
    public class AuditLogRepository : BaseRepository<audit_log>, IAuditLogRepository
    {
        #region Constructors
        public AuditLogRepository(IHustServiceCollection serviceCollection) : base(serviceCollection)
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Lấy danh sách lịch sử truy cập (lọc và phân trang)
        /// </summary>
        /// <param name="searchFilter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public async Task<FilterResult<AuditLog>> GetLogsByFilter(string userId, string searchFilter, 
            int? pageIndex, int? pageSize, DateTime? dateFrom, DateTime? dateTo)
        {
            // Thiết lập các tham số
            var parameters = new DynamicParameters();
            parameters.Add("$UserId", userId);
            parameters.Add("$SearchFilter", searchFilter);
            parameters.Add("$PageIndex", pageIndex);
            parameters.Add("$PageSize", pageSize);
            parameters.Add("$DateFrom", dateFrom);
            parameters.Add("$DateTo", dateTo);


            // Kết quả đầu ra
            parameters.Add("$TotalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("$TotalPage", dbType: DbType.Int32, direction: ParameterDirection.Output);

            IEnumerable<audit_log> res; 
            using (var connection = await this.CreateConnectionAsync())
            {
                res = await connection.QueryAsync<audit_log>(
                    sql: $"Proc_Log_GetLogsFilterPaging",
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: ConnectionTimeout);
            }

            // Trả về kết quả filter
            return new FilterResult<AuditLog>
            {
                TotalPages = parameters.Get<int?>("$TotalPage"),
                TotalRecords = parameters.Get<int?>("$TotalRecord"),
                Data = res != null ? this.ServiceCollection.Mapper.Map<IEnumerable<AuditLog>>(res) : null
            };
        }
        #endregion

    }
}
