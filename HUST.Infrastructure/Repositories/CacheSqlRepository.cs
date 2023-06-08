using HUST.Core.Interfaces.Repository;
using HUST.Core.Models.Entity;
using HUST.Core.Utils;

namespace HUST.Infrastructure.Repositories
{
    public class CacheSqlRepository : BaseRepository<cache_sql>, ICacheSqlRepository
    {
        #region Constructors
        public CacheSqlRepository(IHustServiceCollection serviceCollection) : base(serviceCollection)
        {

        }

        #endregion

        #region Methods
        #endregion

    }
}
