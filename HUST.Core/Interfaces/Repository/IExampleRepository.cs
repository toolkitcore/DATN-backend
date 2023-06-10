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
    }
}
