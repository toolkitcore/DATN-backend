using Dapper;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Models.Entity;
using HUST.Core.Utils;
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
        
        #endregion

    }
}
