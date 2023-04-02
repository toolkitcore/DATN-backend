using System;

namespace HUST.Core.Models
{
    /// <summary>
    /// Lớp cơ sở cho các thực thể
    /// </summary>
    public class BaseEntity : BaseModel
    {
        #region Properties

        /// <summary>
        /// Thời điểm tạo
        /// </summary>
        public DateTime? created_date { get; set; }

        /// <summary>
        /// Thời điểm cập nhật
        /// </summary>
        public DateTime? modified_date { get; set; }

        #endregion
    }
}
