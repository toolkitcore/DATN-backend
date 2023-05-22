using HUST.Core.Models.ServerObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Service
{
    /// <summary>
    /// Interface quy định các service xử lý nghiệp vụ khi thao tác dữ liệu
    /// </summary>
    /// <typeparam name="TEntity">Lớp thực thể</typeparam>
    public interface IBaseService<TEntity> where TEntity : class
    {
        /// <summary>
        /// Service xử lý khi lấy dữ liệu theo Khóa chính (id)
        /// </summary>
        /// <param name="entityId">Khóa chính</param>
        /// <returns>Đối tượng ServiceResult chứa kết quả thực hiện</returns>
        Task<ServiceResult> GetById(Guid entityId);

        /// <summary>
        /// Service xử lý khi thêm dữ liệu
        /// </summary>
        /// <param name="entity">Đối tượng muốn thêm</param>
        /// <returns>Đối tượng ServiceResult chứa kết quả thực hiện</returns>
        Task<ServiceResult> Insert(TEntity entity);

        /// <summary>
        /// Service xử lý khi cập nhật/chỉnh sửa dữ liệu
        /// </summary>
        /// <param name="entityId">Khóa chính</param>
        /// <param name="entity">Đối tượng muốn cập nhật</param>
        /// <returns>Đối tượng ServiceResult chứa kết quả thực hiện</returns>
        Task<ServiceResult> Update(Guid entityId, TEntity entity);

        /// <summary>
        /// Service xử lý khi xóa dữ liệu theo Khóa chính (id)
        /// </summary>
        /// <param name="entityId">Khóa chính</param>
        /// <returns>Đối tượng ServiceResult chứa kết quả thực hiện</returns>
        Task<ServiceResult> Delete(Guid entityId);
    }
}
