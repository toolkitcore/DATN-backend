using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng cache_sql: Bảng thông tin cache_sql
    /// </summary>
    public class CacheSql : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public int CacheSqlId { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public string CacheKey { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Id user
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Loại cache
        /// </summary>
        public int? CacheType { get; set; }

        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Thời gian hết hạn
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}
