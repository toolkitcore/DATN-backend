using HUST.Core.Interfaces.Repository;
using HUST.Core.Models.Entity;
using HUST.Core.Utils;

namespace HUST.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<user>, IUserRepository
    {
        #region Constructors
        public UserRepository(IHustServiceCollection serviceCollection) : base(serviceCollection)
        {

        }
        #endregion

    }
}
