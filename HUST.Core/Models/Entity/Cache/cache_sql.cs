using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng cache_sql: Bảng thông tin cache_sql
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("cache_sql")]
    public class cache_sql : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public int cache_sql_id { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public string cache_key { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// Id user
        /// </summary>
        public Guid? user_id { get; set; }

        /// <summary>
        /// Loại cache
        /// </summary>
        public int? cache_type { get; set; }

        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        public DateTime? start_time { get; set; }

        /// <summary>
        /// Thời gian hết hạn
        /// </summary>
        public DateTime? end_time { get; set; }
    }
}
