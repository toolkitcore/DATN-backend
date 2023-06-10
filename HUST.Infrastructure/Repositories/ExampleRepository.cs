using Dapper;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using HUST.Core.Models.Param;
using HUST.Core.Utils;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace HUST.Infrastructure.Repositories
{
    public class ExampleRepository : BaseRepository<example>, IExampleRepository
    {
        #region Constructors
        public ExampleRepository(IHustServiceCollection serviceCollection) : base(serviceCollection)
        {

        }

        #endregion

        #region Methods
        /// <summary>
        /// Tìm kiếm example
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<List<Example>> SearchExample(SearchExampleParam param)
        {
            using (var connection = await this.CreateConnectionAsync())
            {
                var parameters = new DynamicParameters();
                parameters.Add("$DictionaryId", param.DictionaryId);
                parameters.Add("$Keyword", param.Keyword);
                parameters.Add("$ToneId", param.ToneId);
                parameters.Add("$ModeId", param.ModeId);
                parameters.Add("$RegisterId", param.RegisterId);
                parameters.Add("$NuanceId", param.NuanceId);
                parameters.Add("$DialectId", param.DialectId);

                string strListLinkedConceptId = null;
                if(param.ListLinkedConceptId != null && param.ListLinkedConceptId.Count >= 0)
                {
                    strListLinkedConceptId = SerializeUtil.SerializeObject(param.ListLinkedConceptId);
                }
                parameters.Add("$ListLinkedConceptId", strListLinkedConceptId);
                parameters.Add("$IsSearchUndecided", param.IsSearchUndecided);
                parameters.Add("$IsFulltextSearch", param.IsFulltextSearch);

                var res = await connection.QueryAsync<example>(
                    sql: "Proc_Example_SearchExample",
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: ConnectionTimeout);

                if (res != null)
                {
                    return this.ServiceCollection.Mapper.Map<List<Example>>(res);
                }

                return new List<Example>();
            }
        }
        #endregion

    }
}
