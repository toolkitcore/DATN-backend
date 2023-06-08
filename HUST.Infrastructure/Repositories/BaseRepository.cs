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
using HUST.Core.Utils;
using System.Text;
using HUST.Core.Models.Entity;
using System.ComponentModel;
using System.Linq;

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
        public readonly string ConnectionString;

        /// <summary>
        /// Thông tin thời gian timeout
        /// </summary>
        public readonly int ConnectionTimeout = GlobalConfig.ConnectionTimeout;

        /// <summary>
        /// Tên lớp entity
        /// </summary>
        protected readonly string TableName;

        /// <summary>
        /// Đối tượng ServiceCollection
        /// </summary>
        protected IHustServiceCollection ServiceCollection;

        #endregion

        #region Constructor
        public BaseRepository(IHustServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;

            // lấy ra thông tin kết nối:
            ConnectionString = serviceCollection.ConfigUtil.GetConnectionString(ConnectionStringSettingKey.Database);

            // Lấy ra tên lớp entity (tên bảng)
            TableName = typeof(TEntity).Name;
        }
        #endregion

        #region Methods

        #region Tạo kết nối
        public IDbConnection CreateConnection()
        {
            var conn = new MySqlConnection(ConnectionString);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            return conn;
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var conn = new MySqlConnection(ConnectionString);
            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync().ConfigureAwait(false);
            }
            return conn;
        }
        #endregion

        #region Get
        public async Task<TEntity> Get(Guid entityId, IDbTransaction dbTransaction)
        {
            var connection = dbTransaction.Connection;
            return await connection.GetAsync<TEntity>(entityId, dbTransaction, ConnectionTimeout);
        }

        public async Task<TEntity> Get(Guid entityId)
        {
            using(var connection = new MySqlConnection(ConnectionString))
            {
                return await connection.GetAsync<TEntity>(entityId, commandTimeout: ConnectionTimeout);
            }
        }
        #endregion

        #region Insert
        public async Task<bool> Insert(TEntity entity, IDbTransaction dbTransaction)
        {
            var connection = dbTransaction.Connection;
            await connection.InsertAsync(entity, dbTransaction, ConnectionTimeout);
            return true;
        }
        public async Task<bool> Insert(IEnumerable<TEntity> entities, IDbTransaction dbTransaction)
        {
            var connection = dbTransaction.Connection;
            var result = await connection.InsertAsync(entities, dbTransaction, ConnectionTimeout); // Cần kiểm nghiệm
            return result > 0;
        }

        public async Task<bool> Insert(TEntity entity)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                // hàm này trả về id kiểu int của bản ghi được insert, chứ không phải số bản ghi insert thành công
                // nếu id không phải kiểu int => return 0
                await connection.InsertAsync(entity, commandTimeout: ConnectionTimeout); 
                return true;
            }
        }

        public async Task<bool> Insert(IEnumerable<TEntity> entities)
        {
            using(var connection = new MySqlConnection(ConnectionString))
            {
                var result = await connection.InsertAsync(entities, commandTimeout: ConnectionTimeout);
                return result > 0;
            }
        }

        public async Task<bool> Insert<T>(T entity, IDbTransaction dbTransaction = null) where T : class
        {
            if (dbTransaction != null)
            {
                var connection = dbTransaction.Connection;
                await connection.InsertAsync(entity, dbTransaction, ConnectionTimeout);
            }
            else
            {
                using (var conn = await this.CreateConnectionAsync())
                {
                    await conn.InsertAsync(entity, dbTransaction, ConnectionTimeout);
                }
            }
            return true;
        }
        public async Task<bool> Insert<T>(IEnumerable<T> entities, IDbTransaction dbTransaction = null)
        {
            if (dbTransaction != null)
            {
                var connection = dbTransaction.Connection;
                var result = await connection.InsertAsync(entities, dbTransaction, ConnectionTimeout); // Cần kiểm nghiệm
                return result > 0;
            }
            else
            {
                using (var conn = await this.CreateConnectionAsync())
                {
                    var result = await conn.InsertAsync(entities, dbTransaction, ConnectionTimeout); // Cần kiểm nghiệm
                    return result > 0;
                }
            }
        }
        #endregion

        #region Update
        public async Task<bool> Update(TEntity entity, IDbTransaction dbTransaction)
        {
            var connection = dbTransaction.Connection;
            return await connection.UpdateAsync(entity, dbTransaction, ConnectionTimeout);
        }

        public async Task<bool> Update(IEnumerable<TEntity> entities, IDbTransaction dbTransaction)
        {
            var connection = dbTransaction.Connection;
            return await connection.UpdateAsync(entities, dbTransaction, ConnectionTimeout);
        }

        public async Task<bool> Update(TEntity entity)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                return await connection.UpdateAsync(entity, commandTimeout: ConnectionTimeout);
            }
        }

        public async Task<bool> Update(IEnumerable<TEntity> entities)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                return await connection.UpdateAsync(entities, commandTimeout: ConnectionTimeout);
            }
        }

        /// <summary>
        /// Updates table T with the values in param.
        /// The table must have a key named "Id" and the value of id must be included in the "param" anon object. The Id value is used as the "where" clause in the generated SQL
        /// </summary>
        /// <param name="type">Type to update. Translates to table name</param>
        /// <param name="param">An anonymous object with key=value types</param>
        /// <param name="transaction">transaction</param>
        /// <returns>true/false</returns>
        public async Task<bool> Update(Type type, object param, IDbTransaction transaction = null)
        {
            var props = new List<string>();
            object id = null;
            var keyAttribute = FunctionUtil.GetEntityKeyProperty(type);

            if(keyAttribute == null)
            {
                return false;
            }

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(param))
            {
                if (!keyAttribute.Name.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    props.Add(property.Name);
                } 
                else
                {
                    id = property.GetValue(param);
                }
            }

            if (id != null && props.Count > 0)
            {
                var sql = string.Format(
                    "UPDATE {0} SET {1} WHERE {2}={3}", 
                    type.Name,
                    string.Join(",", props.Select(prop => $"{prop}=@{prop}")), 
                    keyAttribute.Name,
                    $"@{keyAttribute.Name}");

                if(transaction != null)
                {
                    var result = await transaction.Connection.ExecuteAsync(sql, param, transaction, ConnectionTimeout) > 0;
                    return result;
                } else
                {
                    using (var conn = await this.CreateConnectionAsync())
                    {
                        var result = await conn.ExecuteAsync(sql, param, commandTimeout: ConnectionTimeout) > 0;
                        return result;
                    }
                }
                
            }
            return false;
        }

        public async Task<bool> Update<T>(object param, IDbTransaction transaction = null)
        {
            return await Update(typeof(T), param, transaction);
        }

        public async Task<bool> Update(object param, IDbTransaction transaction = null)
        {
            return await Update(typeof(TEntity), param, transaction);
        }
        #endregion

        #region Delete
        public async Task<bool> Delete(TEntity entity, IDbTransaction dbTransaction)
        {
            var connection = dbTransaction.Connection;
            return await connection.DeleteAsync(entity, dbTransaction, ConnectionTimeout);
        }

        public async Task<bool> Delete(IEnumerable<TEntity> entities, IDbTransaction dbTransaction)
        {
            var connection = dbTransaction.Connection;
            return await connection.DeleteAsync(entities, dbTransaction, ConnectionTimeout);
        }

        public async Task<bool> Delete(TEntity entity)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                return await connection.DeleteAsync(entity, commandTimeout: ConnectionTimeout);
            }
        }

        public async Task<bool> Delete(IEnumerable<TEntity> entities)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                return await connection.DeleteAsync(entities, commandTimeout: ConnectionTimeout);
            }
        }

        public async Task<bool> Delete(Type entityTable, object param, IDbTransaction transaction = null)
        {
            var whereList = new List<string>();
            var parameters = new DynamicParameters();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(param))
            {
                whereList.Add($"{property.Name} = @{property.Name}");
            }
            parameters.AddDynamicParams(param);

            // Khai báo câu lệnh sql
            var sqlCommand = $"DELETE FROM {entityTable.Name} WHERE {string.Join(" AND ", whereList)}";

            if (transaction != null)
            {
                var result = await transaction.Connection.ExecuteAsync(sqlCommand, param, transaction, ConnectionTimeout) > 0;
                return result;
            }

            using (var conn = await this.CreateConnectionAsync())
            {
                var result = await conn.ExecuteAsync(sqlCommand, param, transaction, ConnectionTimeout) > 0;
                return result;
            }
        }

        public async Task<bool> Delete<T>(object param, IDbTransaction transaction = null)
        {
            return await Delete(typeof(T), param, transaction);
        }

        public async Task<bool> Delete(object param, IDbTransaction transaction = null)
        {
            return await Delete(typeof(TEntity), param, transaction);
        }
        #endregion

        #region Check duplicate
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

            using (var conn = await this.CreateConnectionAsync())
            {
                // Thực hiện truy vấn
                var result = await conn.QueryFirstOrDefaultAsync<object>(
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

            using (var conn = await this.CreateConnectionAsync())
            {
                // Thực hiện truy vấn
                var result = await conn.QueryFirstOrDefaultAsync<object>(
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
        }
        #endregion

        #region Select
        public async Task<object> SelectObject<T>(Dictionary<string, object> paramDict)
        {
            var entityTable = FunctionUtil.GetEntityType<T>();
            var whereList = new List<string>();
            var parameters = new DynamicParameters();
            foreach (var item in paramDict)
            {
                whereList.Add($"{item.Key} = @{item.Key}");
                parameters.Add($"@{item.Key}", item.Value);
            }

            // Khai báo câu lệnh sql
            var sqlCommand = $"SELECT * FROM {entityTable.Name} WHERE {string.Join(" AND ", whereList.ToArray())}";

            using (var conn = await this.CreateConnectionAsync())
            {
                // Thực hiện truy vấn
                var result = await conn.QueryFirstOrDefaultAsync<object>(
                    sql: sqlCommand,
                    param: parameters,
                    commandType: CommandType.Text);

                if (result != null)
                {
                    var data = SerializeUtil.DeserializeObject(SerializeUtil.SerializeObject(result), entityTable);
                    return ServiceCollection.Mapper.Map<T>(data);
                }

                return null;
            }

        }
        public async Task<object> SelectObject<T>(object param)
        {
            var entityTable = FunctionUtil.GetEntityType<T>();
            var whereList = new List<string>();
            var parameters = new DynamicParameters();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(param))
            {
                whereList.Add($"{property.Name} = @{property.Name}");
            }
            parameters.AddDynamicParams(param);

            // Khai báo câu lệnh sql
            var sqlCommand = $"SELECT * FROM {entityTable.Name} WHERE {string.Join(" AND ", whereList)}";

            using (var conn = await this.CreateConnectionAsync())
            {
                // Thực hiện truy vấn
                var result = await conn.QueryFirstOrDefaultAsync<object>(
                    sql: sqlCommand,
                    param: parameters,
                    commandType: CommandType.Text);


                if (result != null)
                {
                    var data = SerializeUtil.DeserializeObject(SerializeUtil.SerializeObject(result), entityTable);
                    return ServiceCollection.Mapper.Map<T>(data);
                }

                return null;
            }
        }
        public async Task<IEnumerable<T>> SelectObjects<T>(Dictionary<string, object> paramDict)
        {
            var entityTable = FunctionUtil.GetEntityType<T>();
            var whereList = new List<string>();
            var parameters = new DynamicParameters();
            foreach (var item in paramDict)
            {
                whereList.Add($"{item.Key} = @{item.Key}");
                parameters.Add($"@{item.Key}", item.Value);
            }

            // Khai báo câu lệnh sql
            var sqlCommand = $"SELECT * FROM {entityTable.Name} WHERE {string.Join(" AND ", whereList)}";

            using (var conn = await this.CreateConnectionAsync())
            {
                // Thực hiện truy vấn
                var result = await conn.QueryAsync<object>(
                    sql: sqlCommand,
                    param: parameters,
                    commandType: CommandType.Text);


                if (result != null)
                {
                    var data = SerializeUtil.DeserializeObject(SerializeUtil.SerializeObject(result),
                        typeof(IEnumerable<>).MakeGenericType(entityTable));
                    return ServiceCollection.Mapper.Map<IEnumerable<T>>(data);
                }

                return default;
            }
        }
        public async Task<IEnumerable<T>> SelectObjects<T>(object param)
        {
            var entityTable = FunctionUtil.GetEntityType<T>();
            var whereList = new List<string>();
            var parameters = new DynamicParameters();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(param))
            {
                whereList.Add($"{property.Name} = @{property.Name}");
            }
            parameters.AddDynamicParams(param);

            // Khai báo câu lệnh sql
            var sqlCommand = $"SELECT * FROM {entityTable.Name} WHERE {string.Join(" AND ", whereList)}";

            using (var conn = await this.CreateConnectionAsync())
            {
                // Thực hiện truy vấn
                var result = await conn.QueryAsync<object>(
                    sql: sqlCommand,
                    param: parameters,
                    commandType: CommandType.Text);


                if (result != null)
                {
                    var data = SerializeUtil.DeserializeObject(SerializeUtil.SerializeObject(result),
                        typeof(IEnumerable<>).MakeGenericType(entityTable));
                    return ServiceCollection.Mapper.Map<IEnumerable<T>>(data);
                }

                return default;
            }
        }
        public async Task<object> SelectManyObjects(string[] tableNames, Dictionary<string, Dictionary<string, object>> paramDict)
        {
            var parameters = new DynamicParameters();
            var sqlCommand = new StringBuilder();
            foreach (var table in tableNames)
            {
                var tableParam = paramDict[table];
                var whereList = new List<string>();
                foreach (var (key, value) in tableParam)
                {
                    parameters.Add($"@{table}.{key}", value);
                    whereList.Add($"{key} = @{table}.{key}");
                }
                sqlCommand.Append($"SELECT * FROM {table} WHERE {string.Join(" AND ", whereList)};");
            }

            using (var conn = await this.CreateConnectionAsync())
            {
                // Thực hiện truy vấn
                var queryResult = await conn.QueryMultipleAsync(
                    sql: sqlCommand.ToString(),
                    param: parameters,
                    commandType: CommandType.Text);

                if (queryResult != null)
                {
                    var res = new Dictionary<string, object>();
                    foreach (var table in tableNames)
                    {
                        var data = await queryResult.ReadAsync();
                        var dataCast = SerializeUtil.DeserializeObject(SerializeUtil.SerializeObject(data),
                        typeof(List<>).MakeGenericType(FunctionUtil.GetModelType(table)));
                        res.Add(table, dataCast);
                    }

                    return res;
                }

                return null;
            }

        }
        #endregion

        #endregion
    }
}
