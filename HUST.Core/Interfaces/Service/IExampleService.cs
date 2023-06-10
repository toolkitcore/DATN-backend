using HUST.Core.Models.DTO;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Service
{
    /// <summary>
    /// Service xử lý example
    /// </summary>
    public interface IExampleService
    {
        /// <summary>
        /// Thêm 1 example vào từ điển
        /// </summary>
        /// <param name="example"></param>
        /// <returns></returns>
        Task<IServiceResult> AddExample(Example example);

        /// <summary>
        /// Thực hiện cập nhật example
        /// </summary>
        /// <param name="example"></param>
        /// <returns></returns>
        Task<IServiceResult> UpdateExample(Example example);

        /// <summary>
        /// Thực hiện xóa example
        /// </summary>
        /// <param name="exampleId"></param>
        /// <returns></returns>
        Task<IServiceResult> DeleteExample(Guid exampleId);

        /// <summary>
        /// Lấy dữ liệu example
        /// </summary>
        /// <param name="exampleId"></param>
        /// <returns></returns>
        Task<IServiceResult> GetExample(Guid exampleId);

        /// <summary>
        /// Tìm kiếm example
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IServiceResult> SearchExample(SearchExampleParam param);
    }
}
