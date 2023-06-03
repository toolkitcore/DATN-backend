using Dapper;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using HUST.Core.Utils;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace HUST.Infrastructure.Repositories
{
    public class ConceptRelationshipRepository : BaseRepository<concept_relationship>, IConceptRelationshipRepository
    {
        #region Constructors
        public ConceptRelationshipRepository(IHustServiceCollection serviceCollection) : base(serviceCollection)
        {

        }

        #endregion

        #region Methods
        #endregion

    }
}
