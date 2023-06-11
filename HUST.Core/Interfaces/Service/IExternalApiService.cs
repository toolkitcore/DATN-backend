using HUST.Core.Models.DTO;
using HUST.Core.Models.ServerObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace HUST.Core.Interfaces.Service
{
    /// <summary>
    /// Service xử lý nghiệp vụ cần call external api
    /// </summary>
    public interface IExternalApiService
    {
        /// <summary>
        /// Lấy kết quả request wordsapi
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        Task<IServiceResult> GetWordsapiResult(string word);
    }
}
