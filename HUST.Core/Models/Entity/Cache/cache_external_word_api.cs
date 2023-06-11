using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    /// <summary>
    /// Bảng cache_external_word_api: Bảng thông tin cache_external_word_api
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("cache_external_word_api")]
    public class cache_external_word_api : BaseEntity
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [System.ComponentModel.DataAnnotations.Key, ExplicitKey]
        public int id { get; set; }

        /// <summary>
        /// Từ
        /// </summary>
        public string word { get; set; }

        /// <summary>
        /// Route api
        /// </summary>
        public string route { get; set; }

        /// <summary>
        /// Loại api
        /// </summary>
        public int external_api_type { get; set; }

        /// <summary>
        /// Giá trị cache
        /// </summary>
        public string value { get; set; }
    }
}
