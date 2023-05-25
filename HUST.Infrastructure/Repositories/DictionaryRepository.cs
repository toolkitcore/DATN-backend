using Dapper;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Models.Entity;
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
                _ = await transaction.Connection.ExecuteAsync(
                    sql: storeName,
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: ConnectionTimeout);
                return true;
            }
        }
        #endregion

    }
}
