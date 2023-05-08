using Dapper;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Models.Entity;
using HUST.Core.Utils;
using System.Data;
using System.Threading.Tasks;

namespace HUST.Infrastructure.Repositories
{
    public class ConceptRepository : BaseRepository<concept>, IConceptRepository
    {
        #region Constructors
        public ConceptRepository(IHustServiceCollection serviceCollection) : base(serviceCollection)
        {

        }

        #endregion

        #region Methods
        
        #endregion

    }
}
