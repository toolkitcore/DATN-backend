using HUST.Core.Interfaces.Repository;
using HUST.Core.Models.Entity;
using HUST.Core.Utils;

namespace HUST.Infrastructure.Repositories
{
    public class CacheExternalWordApiRepository : BaseRepository<cache_external_word_api>, ICacheExternalWordApiRepository
    {
        #region Constructors
        public CacheExternalWordApiRepository(IHustServiceCollection serviceCollection) : base(serviceCollection)
        {

        }

        #endregion

        #region Methods
        #endregion

    }
}
