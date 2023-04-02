using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Repository
{
    /// <summary>
    /// Interface quy định các thao tác lấy dữ liệu cơ bản
    /// </summary>
    /// <typeparam name="TEntity">Lớp thực thể</typeparam>
    public interface IBaseRepository<TEntity> where TEntity: class
    {
        /// <summary>
        /// Lấy ra đối tượng thực thể theo khóa chính(id)
        /// </summary>
        /// <param name="entityId">Khóa chính</param>
        /// <returns>Đối tượng thực thể</returns>
        Task<TEntity> Get(Guid entityId, IDbTransaction dbTransaction = null);

        /// <summary>
        /// Thêm mới đối tượng thực thể
        /// </summary>
        /// <param name="entity">Đối tượng cần thêm mới</param>
        /// <returns>Số bản ghi được thêm</returns>
        Task<bool> Insert(TEntity entity, IDbTransaction dbTransaction = null);

        /// <summary>
        /// Thêm mới đối tượng thực thể
        /// </summary>
        /// <param name="entity">Đối tượng cần thêm mới</param>
        /// <returns>Số bản ghi được thêm</returns>
        Task<bool> Insert(IEnumerable<TEntity> entities, IDbTransaction dbTransaction = null);

        /// <summary>
        /// Chỉnh sửa đối tượng thực thể
        /// </summary>
        /// <param name="entity">Đối tượng cần chỉnh sửa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        Task<bool> Update(TEntity entity, IDbTransaction dbTransaction = null);

        /// <summary>
        /// Thêm mới đối tượng thực thể
        /// </summary>
        /// <param name="entity">Đối tượng cần thêm mới</param>
        /// <returns>Số bản ghi được thêm</returns>
        Task<bool> Update(IEnumerable<TEntity> entities, IDbTransaction dbTransaction = null);


        /// <summary>
        /// Xóa đối tượng thực thể theo khóa chính(id)
        /// </summary>
        /// <param name="entityId">Khóa chính</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        Task<bool> Delete(TEntity entity, IDbTransaction dbTransaction = null);

        /// <summary>
        /// Xóa đối tượng thực thể theo khóa chính(id)
        /// </summary>
        /// <param name="entityId">Khóa chính</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        Task<bool> Delete(IEnumerable<TEntity> entities, IDbTransaction dbTransaction = null);
    }
}
