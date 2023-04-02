using System.Collections.Generic;

namespace HUST.Core.Models.ServerObject
{
    /// <summary>
    /// Kết quả lọc/ phân trang dữ liệu
    /// </summary>
    /// <typeparam name="TModel">Đối tượng thực thể</typeparam>
    public class FilterResult<TModel>
    {
        #region Properties

        /// <summary>
        /// Tổng số bản ghi trả về
        /// </summary>
        public int? TotalRecords { get; set; }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int? TotalPages { get; set; }

        /// <summary>
        /// Danh sách bản ghi trả về
        /// </summary>
        public IEnumerable<TModel> Data { get; set; }

        #endregion
    }
}
