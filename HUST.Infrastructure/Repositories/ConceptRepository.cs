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
    public class ConceptRepository : BaseRepository<concept>, IConceptRepository
    {
        #region Constructors
        public ConceptRepository(IHustServiceCollection serviceCollection) : base(serviceCollection)
        {

        }

        #endregion

        #region Methods
        /// <summary>
        /// Thực hiện lấy danh sách concept trong từ điển mà khớp với xâu tìm kiếm của người dùng
        /// </summary>
        /// <param name="searchKey"></param>
        /// <param name="dictionaryId"></param>
        /// <param name="isSearchSoundex"></param>
        /// <returns></returns>
        public async Task<List<Concept>> SearchConcept(string searchKey, string dictionaryId, bool? isSearchSoundex)
        {
            using (var connection = await this.CreateConnectionAsync())
            {
                var parameters = new DynamicParameters();
                parameters.Add("$SearchKey", searchKey);
                parameters.Add("$DictionaryId", dictionaryId);
                parameters.Add("$IsSearchSoundex", isSearchSoundex);

                var res = await connection.QueryAsync<concept>(
                    sql: "Proc_Concept_SearchConcept",
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: ConnectionTimeout);

                if(res != null)
                {
                    return this.ServiceCollection.Mapper.Map<List<Concept>>(res);
                }

                return new List<Concept>();
            }
        }
        #endregion

    }
}
