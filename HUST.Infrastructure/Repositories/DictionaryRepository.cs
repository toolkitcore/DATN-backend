using Dapper;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Models.Entity;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using System;
using System.Data;
using System.Threading.Tasks;

namespace HUST.Infrastructure.Repositories
{
    public class DictionaryRepository : BaseRepository<dictionary>, IDictionaryRepository
    {
        #region Constructors
        public DictionaryRepository(IHustServiceCollection serviceCollection) : base(serviceCollection)
        {

        }

        #endregion

        #region Methods
        /// <summary>
        /// Thực hiện clone dữ liệu từ điển (có xóa dữ liệu từ điển đích)
        /// </summary>
        /// <param name="sourceDictionaryId"></param>
        /// <param name="destDictionaryId"></param>
        /// <returns></returns>
        public async Task<bool> CloneDictionaryData(Guid sourceDictionaryId, Guid destDictionaryId, IDbTransaction transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("$SourceDictionaryId", sourceDictionaryId);
            parameters.Add("$DestDictionaryId", destDictionaryId);

            var storeName = "Proc_Dictionary_CloneDictionaryData";
            if (transaction != null)
            {
                _ = await transaction.Connection.ExecuteAsync(
                    sql: storeName,
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    transaction: transaction,
                    commandTimeout: ConnectionTimeout);
                return true;
            } 

            using (var connection = await this.CreateConnectionAsync())
            {
                _ = await connection.ExecuteAsync(
                    sql: storeName,
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: ConnectionTimeout);
                return true;
            }
        }

        /// <summary>
        /// Thực hiện xóa dữ liệu trong 1 từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteDictionaryData(Guid dictionaryId, IDbTransaction transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("$DictionaryId", dictionaryId);

            var storeName = "Proc_Dictionary_DeleteDictionaryData";
            if (transaction != null)
            {
                _ = await transaction.Connection.ExecuteAsync(
                    sql: storeName,
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    transaction: transaction,
                    commandTimeout: ConnectionTimeout);
                return true;
            }

            using (var connection = await this.CreateConnectionAsync())
            {
                _ = await connection.ExecuteAsync(
                    sql: storeName,
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: ConnectionTimeout);
                return true;
            }
        }

        /// <summary>
        /// Thực hiện copy dữ liệu từ từ điển nguồn và gộp vào dữ liệu ở từ điển đích
        /// </summary>
        /// <param name="sourceDictionaryId"></param>
        /// <param name="destDictionaryId"></param>
        /// <param name="isDeleteData"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<bool> TransferDictionaryData(Guid sourceDictionaryId, Guid destDictionaryId, bool isDeleteData, IDbTransaction transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("$SourceDictionaryId", sourceDictionaryId);
            parameters.Add("$DestDictionaryId", destDictionaryId);
            parameters.Add("$IsDeleteData", isDeleteData);

            var storeName = "Proc_Dictionary_TransferDictionaryData";
            if (transaction != null)
            {
                _ = await transaction.Connection.ExecuteAsync(
                    sql: storeName,
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    transaction: transaction,
                    commandTimeout: ConnectionTimeout);
                return true;
            }

            using (var connection = await this.CreateConnectionAsync())
            {
                _ = await connection.ExecuteAsync(
                    sql: storeName,
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: ConnectionTimeout);
                return true;
            }
        }

        /// <summary>
        /// Thực hiện lấy số lượng concept, example trong 1 từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        public async Task<DictionaryNumberRecord> GetNumberRecord(Guid dictionaryId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("$DictionaryId", dictionaryId);
            // Kết quả đầu ra
            parameters.Add("$NumberConcept", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("$NumberExample", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var storeName = "Proc_Dictionary_GetNumberRecord";
            using (var connection = await this.CreateConnectionAsync())
            {
                _ = await connection.ExecuteAsync(
                    sql: storeName,
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: ConnectionTimeout);
            }

            // Trả về kết quả filter
            return new DictionaryNumberRecord
            {
                NumberConcept = parameters.Get<int?>("$NumberConcept"),
                NumberExample = parameters.Get<int?>("$NumberExample"),
            };
        }
        #endregion

    }
}
