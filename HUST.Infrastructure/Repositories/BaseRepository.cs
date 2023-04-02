using AutoMapper;
using Dapper;
using HUST.Core.Constants;
using HUST.Core.Interfaces.Repository;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace HUST.Infrastructure.Repositories
{
    /// <summary>
    /// Lớp cơ sở cho việc thao tác với CSDL (truy vấn, thêm, sửa, xóa... dữ liệu)
    /// </summary>
    /// <typeparam name="TEntity">Lớp thực thể</typeparam>
    public class BaseRepository<TEntity>: IBaseRepository<TEntity> where TEntity: class
    {
        #region Fields

        /// <summary>
        /// Thông tin về kết nối
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Tên lớp entity
        /// </summary>
        protected readonly string TableName;

        /// <summary>
        /// Kết nối tới cơ sở dữ liệu
        /// </summary>
        protected IDbConnection Connection = null;

        protected IMapper Mapper;

        #endregion

        #region Constructor
        public BaseRepository(IConfiguration configuration, IMapper mapper)
        {
            // lấy ra thông tin kết nối:
            _connectionString = configuration.GetConnectionString(ConnectionStringSettingKey.Database);

            // Khởi tạo kết nối:
            Connection = new MySqlConnection(_connectionString);

            // Lấy ra tên lớp entity (tên bảng)
            TableName = typeof(TEntity).Name;

            Mapper = mapper;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Lấy thông tin đối tượng theo khóa chính (id)
        /// </summary>
        /// <param name="entityId">Khóa chính (id)</param>
        /// <returns>Đối tượng</returns>
        public async Task<TEntity> Get(Guid entityId, IDbTransaction dbTransaction = null)
        {
            var connection = this.Connection;
            if (dbTransaction != null)
            {
                connection = dbTransaction.Connection;
            }
            return await connection.GetAsync<TEntity>(entityId, dbTransaction);
        }

        /// <summary>
        /// Thêm đối tượng
        /// </summary>
        /// <param name="entity">Đối tượng cần thêm</param>
        /// <returns>Số bản ghi được thêm</returns>
        public async Task<bool> Insert(TEntity entity, IDbTransaction dbTransaction = null)
        {
            var connection = this.Connection;
            if (dbTransaction != null)
            {
                connection = dbTransaction.Connection;
            }
            var result = await connection.InsertAsync(entity, dbTransaction);
            return result > 0;
        }

        /// <summary>
        /// Thêm đối tượng
        /// </summary>
        /// <param name="entity">Đối tượng cần thêm</param>
        /// <returns>Số bản ghi được thêm</returns>
        public async Task<bool> Insert(IEnumerable<TEntity> entities, IDbTransaction dbTransaction = null)
        {
            var connection = this.Connection;
            if (dbTransaction != null)
            {
                connection = dbTransaction.Connection;
            }
            var result = await connection.InsertAsync(entities, dbTransaction);
            return result > 0;
        }

        /// <summary>
        /// Chỉnh sửa thông tin đối tượng
        /// </summary>
        /// <param name="entity">Đối tượng cần cập nhật thông tin</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        public async Task<bool> Update(TEntity entity, IDbTransaction dbTransaction = null)
        {
            var connection = this.Connection;
            if (dbTransaction != null)
            {
                connection = dbTransaction.Connection;
            }
            return await connection.UpdateAsync(entity, dbTransaction);
        }

        /// <summary>
        /// Chỉnh sửa thông tin đối tượng
        /// </summary>
        /// <param name="entity">Đối tượng cần cập nhật thông tin</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        public async Task<bool> Update(IEnumerable<TEntity> entities, IDbTransaction dbTransaction = null)
        {
            var connection = this.Connection;
            if (dbTransaction != null)
            {
                connection = dbTransaction.Connection;
            }
            return await connection.UpdateAsync(entities, dbTransaction);
        }

        /// <summary>
        /// Xóa đối tượng dựa theo khóa chính
        /// </summary>
        /// <param name="entityId">Khóa chính</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        public async Task<bool> Delete(TEntity entity, IDbTransaction dbTransaction = null)
        {
            var connection = this.Connection;
            if (dbTransaction != null)
            {
                connection = dbTransaction.Connection;
            }
            return await connection.DeleteAsync(entity, dbTransaction);
        }

        /// <summary>
        /// Xóa đối tượng dựa theo khóa chính
        /// </summary>
        /// <param name="entityId">Khóa chính</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        public async Task<bool> Delete(IEnumerable<TEntity> entities, IDbTransaction dbTransaction = null)
        {
            var connection = this.Connection;
            if (dbTransaction != null)
            {
                connection = dbTransaction.Connection;
            }
            return await connection.DeleteAsync(entities, dbTransaction);
        }

        /// <summary>
        /// Hàm kiểm tra trùng lặp dữ liệu
        /// </summary>
        /// <param name="propName">Tên thuộc tính (tương ứng với tên trường trong CSDL)</param>
        /// <param name="value">Giá trị muốn kiểm tra</param>
        /// <returns>true - giá trị bị trùng, false - giá trị không bị trùng</returns>
        public async Task<bool> CheckDuplicate(string propName, object value)
        {
            // Khai báo câu lệnh sql
            var sqlCommand = $"SELECT {propName} FROM {TableName} WHERE {propName} = @{propName}";

            // Thiết lập tham số
            var parameters = new DynamicParameters();
            parameters.Add($"@{propName}", value);

            // Thực hiện truy vấn
            var result = await Connection.QueryFirstOrDefaultAsync<object>(
                sql: sqlCommand, 
                param: parameters, 
                commandType: CommandType.Text);

            // Nếu có dữ liệu trả về (khác null) => giá trị prop bị trùng
            if(result != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Hàm kiểm tra trùng lặp dữ liệu trước khi update bản ghi
        /// </summary>
        /// <param name="propName">Tên trường dữ liệu</param>
        /// <param name="value">Giá trị cần kiểm tra</param>
        /// <param name="entityId">Đối tượng thực thể</param>
        /// <returns>true - giá trị bị trùng, false - giá trị không bị trùng</returns>
        public async Task<bool> CheckDuplicateBeforeUpdate(string propName, object value, TEntity entity)
        {
            var entityId = typeof(TEntity).GetProperty($"{TableName}Id").GetValue(entity);

            // Khai báo câu lệnh sql
            // Có thể dùng: AND hoặc &&, <> hoặc != 
            var sqlCommand = $"SELECT {propName} FROM {TableName} WHERE {TableName}Id <> @{TableName}Id AND {propName} = @{propName}";

            // Thiết lập tham số
            var parameters = new DynamicParameters();
            parameters.Add($"@{TableName}Id", entityId);
            parameters.Add($"@{propName}", value);

            // Thực hiện truy vấn
            var result = await Connection.QueryFirstOrDefaultAsync<object>(
                sql: sqlCommand,
                param: parameters,
                commandType: CommandType.Text);


            // Nếu có dữ liệu trả về (khác null) => giá trị prop bị trùng
            if (result != null)
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}
