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

        #region Tạo kết nối
        /// <summary>
        /// Tạo kết nối
        /// </summary>
        /// <returns></returns>
        IDbConnection CreateConnection();

        /// <summary>
        /// Tạo kết nối bất đồng bộ
        /// </summary>
        /// <returns></returns>
        Task<IDbConnection> CreateConnectionAsync();
        #endregion

        #region Get by id
        /// <summary>
        /// Lấy ra đối tượng thực thể theo khóa chính(id)
        /// </summary>
        /// <param name="entityId">Khóa chính</param>
        /// <param name="dbTransaction">transaction</param>
        /// <returns>Đối tượng thực thể</returns>
        Task<TEntity> Get(Guid entityId, IDbTransaction dbTransaction);

        /// <summary>
        /// Lấy ra đối tượng thực thể theo khóa chính(id)
        /// </summary>
        /// <param name="entityId">Khóa chính</param>
        /// <returns>Đối tượng thực thể</returns>
        Task<TEntity> Get(Guid entityId);
        #endregion

        #region Insert
        /// <summary>
        /// Thêm mới đối tượng thực thể
        /// </summary>
        /// <param name="entity">Đối tượng cần thêm mới</param>
        /// <param name="dbTransaction">transaction</param> 
        /// <returns>Kết quả thực hiện</returns>
        Task<bool> Insert(TEntity entity, IDbTransaction dbTransaction);

        /// <summary>
        /// Thêm mới 1 list đối tượng thực thể
        /// </summary>
        /// <param name="entities">Danh sách bản ghi thêm mới</param>
        /// <param name="dbTransaction">transaction</param> 
        /// <returns>Kết quả thực hiện</returns>
        Task<bool> Insert(IEnumerable<TEntity> entities, IDbTransaction dbTransaction);

        /// <summary>
        /// Thêm mới đối tượng thực thể
        /// </summary>
        /// <param name="entity">Đối tượng cần thêm mới</param>
        /// <returns>Kết quả thực hiện</returns>
        Task<bool> Insert(TEntity entity);

        /// <summary>
        /// Thêm mới 1 list đối tượng thực thể
        /// </summary>
        /// <param name="entities">Danh sách bản ghi thêm mới</param>
        /// <returns>Kết quả thực hiện</returns>
        Task<bool> Insert(IEnumerable<TEntity> entities);

        Task<bool> Insert<T>(T entity, IDbTransaction dbTransaction = null) where T : class;
        Task<bool> Insert<T>(IEnumerable<T> entities, IDbTransaction dbTransaction = null);
        #endregion

        #region Update
        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        /// <param name="entity">Đối tượng cần chỉnh sửa</param>
        /// <param name="dbTransaction">transaction</param> 
        /// <returns>Kết quả thực hiện</returns>
        Task<bool> Update(TEntity entity, IDbTransaction dbTransaction);

        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        /// <param name="entities">Danh sách bản ghi cập nhật</param>
        /// <param name="dbTransaction">transaction</param>
        /// <returns>Kết quả thực hiện</returns>
        Task<bool> Update(IEnumerable<TEntity> entities, IDbTransaction dbTransaction);

        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        /// <param name="entity">Đối tượng cần chỉnh sửa</param>
        /// <returns>Kết quả thực hiện</returns>
        Task<bool> Update(TEntity entity);

        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        /// <param name="entities">Danh sách bản ghi cập nhật</param>
        /// <returns>Kết quả thực hiện</returns>
        Task<bool> Update(IEnumerable<TEntity> entities);

        Task<bool> Update(Type type, object param, IDbTransaction transaction = null);

        Task<bool> Update<T>(object param, IDbTransaction transaction = null);

        Task<bool> Update(object param, IDbTransaction transaction = null);
        #endregion

        #region Delete
        /// <summary>
        /// Xóa bản ghi
        /// </summary>
        /// <param name="entity">Bản ghi cần xóa (cần chứa khóa chính)</param>
        /// <param name="dbTransaction">transaction</param>
        /// <returns>Kết quả thực hiện</returns>
        Task<bool> Delete(TEntity entity, IDbTransaction dbTransaction);

        /// <summary>
        /// Xóa nhiều bản ghi
        /// </summary>
        /// <param name="entities">danh sách bản ghi</param>
        /// <param name="dbTransaction">transaction</param>
        /// <returns>Kết quả thực hiện</returns>
        Task<bool> Delete(IEnumerable<TEntity> entities, IDbTransaction dbTransaction);

        /// <summary>
        /// Xóa bản ghi
        /// </summary>
        /// <param name="entity">Bản ghi cần xóa (cần chứa khóa chính)</param>
        /// <returns>Kết quả thực hiện</returns>
        Task<bool> Delete(TEntity entity);

        /// <summary>
        /// Xóa nhiều bản ghi
        /// </summary>
        /// <param name="entities">danh sách bản ghi</param>
        /// <returns>Kết quả thực hiện</returns>
        Task<bool> Delete(IEnumerable<TEntity> entities);

        Task<bool> Delete(Type entityTable, object param, IDbTransaction transaction = null);

        Task<bool> Delete<T>(object param, IDbTransaction transaction = null);

        Task<bool> Delete(object param, IDbTransaction transaction = null);
        #endregion

        #region Check duplicate
        /// <summary>
        /// Hàm kiểm tra trùng lặp dữ liệu
        /// </summary>
        /// <param name="propName">Tên thuộc tính (tương ứng với tên trường trong CSDL)</param>
        /// <param name="value">Giá trị muốn kiểm tra</param>
        /// <returns>true - giá trị bị trùng, false - giá trị không bị trùng</returns>
        Task<bool> CheckDuplicate(string propName, object value);

        /// <summary>
        /// Hàm kiểm tra trùng lặp dữ liệu trước khi update bản ghi
        /// </summary>
        /// <param name="propName">Tên trường dữ liệu</param>
        /// <param name="value">Giá trị cần kiểm tra</param>
        /// <param name="entityId">Đối tượng thực thể</param>
        /// <returns>true - giá trị bị trùng, false - giá trị không bị trùng</returns>
        Task<bool> CheckDuplicateBeforeUpdate(string propName, object value, TEntity entity);
        #endregion

        #region Select
        /// <summary>
        /// Select 1 bản ghi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramDict"></param>
        /// <returns></returns>
        Task<object> SelectObject<T>(Dictionary<string, object> paramDict);

        /// <summary>
        /// Select 1 bản ghi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<object> SelectObject<T>(object param);

        /// <summary>
        /// Select nhiều bản ghi thuộc 1 bảng
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramDict"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> SelectObjects<T>(Dictionary<string, object> paramDict);

        /// <summary>
        /// Select nhiều bản ghi thuộc 1 bảng
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> SelectObjects<T>(object param);

        /// <summary>
        /// Select bản ghi thuộc nhiều bảng
        /// </summary>
        /// <param name="tableNames"></param>
        /// <param name="paramDict"></param>
        /// <returns></returns>
        Task<object> SelectManyObjects(string[] tableNames, Dictionary<string, Dictionary<string, object>> paramDict);
        #endregion

    }
}
