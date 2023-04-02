using HUST.Core.Models.DTO;
using HUST.Core.Models.ServerObject;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Service
{

    public interface IUserService: IBaseService<User>
    {
        Task<ServiceResult> GetUserIdByToken(string token);
    }
}
