using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    /// <summary>
    /// Bảng user: Bảng thông tin user
    /// </summary>
    public class User : BaseDTO
    {
        /// <summary>
        /// Id khóa chính
        /// </summary>
        [Key]
        public Guid UserId { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Tên đầy đủ
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Tên hiển thị
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Vị trí, công việc
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Link avatar
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? Status { get; set; }

        #region Custom
        /// <summary>
        /// Id dictionary đang thao tác
        /// </summary>
        public Guid? DictionaryId { get; set; }

        #endregion
    }
}
