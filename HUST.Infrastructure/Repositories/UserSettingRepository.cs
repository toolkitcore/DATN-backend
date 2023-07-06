using HUST.Core.Interfaces.Repository;
using HUST.Core.Models.Entity;
using HUST.Core.Utils;

namespace HUST.Infrastructure.Repositories
{
    public class UserSettingRepository : BaseRepository<user_setting>, IUserSettingRepository
    {
        #region Constructors
        public UserSettingRepository(IHustServiceCollection serviceCollection) : base(serviceCollection)
        {

        }

        #endregion

        #region Methods
        #endregion

    }
}
