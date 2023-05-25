using Dapper;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Models.Entity;
using HUST.Core.Utils;
using System.Data;
using System.Threading.Tasks;

namespace HUST.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<user>, IUserRepository
    {
        #region Constructors
        public UserRepository(IHustServiceCollection serviceCollection) : base(serviceCollection)
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Thực hiện khởi tạo dữ liệu tài khoản khi tài khoản được kích hoạt
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> CreateActivatedAccountData(string userId)
        {
            using (var connection = await this.CreateConnectionAsync())
            {
                var parameters = new DynamicParameters();
                parameters.Add("$UserId", userId);
                var res = await connection.ExecuteAsync(
                    sql: $"Proc_Account_CreateActivatedAccountData",
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: ConnectionTimeout);
                return true;
            }
        }
        #endregion

    }
}
