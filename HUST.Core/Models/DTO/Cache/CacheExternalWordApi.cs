using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng cache_external_word_api: Bảng thông tin cache_external_word_api
    /// </summary>
    public class CacheExternalWordApi : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Từ
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// Route api
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Loại api
        /// </summary>
        public int ExternalApiType { get; set; }

        /// <summary>
        /// Giá trị cache
        /// </summary>
        public string Value { get; set; }
    }
}
