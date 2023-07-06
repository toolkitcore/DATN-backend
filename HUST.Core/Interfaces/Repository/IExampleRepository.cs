using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using HUST.Core.Models.Param;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Repository
{

    public interface IExampleRepository: IBaseRepository<example>
    {
        /// <summary>
        /// Tìm kiếm example
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<List<Example>> SearchExample(SearchExampleParam param);

        /// <summary>
        /// Thực hiện lấy danh sách top example thêm mới gần đây nhất
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<List<Example>> GetListMostRecentExample(string dictionaryId, int limit);
    }
}
