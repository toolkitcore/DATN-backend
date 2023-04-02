using HUST.Core.Models.DTO;
using System;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Repository
{

    public interface IUserRepository: IBaseRepository<User>
    {
        public Task<Guid> GetUserId(User user);
    }
}
