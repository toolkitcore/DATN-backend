using AutoMapper;
using Dapper;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Models.DTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;

namespace HUST.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        #region Constructors
        public UserRepository(IConfiguration configuration, IMapper mapper) : base(configuration, mapper)
        {

        }
        #endregion

        public async Task<Guid> GetUserId(User user)
        {
            return await Connection.QueryFirstOrDefaultAsync<Guid>(
                sql: "Proc_GetUserId",
                param: user,
                commandType: CommandType.StoredProcedure);
        }
    }
}
