using HUST.Core.Models.DTO;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Service
{
    /// <summary>
    /// Service xử lý template xuất khẩu, nhập khẩu
    /// </summary>
    public interface ITemplateService
    {
        /// <summary>
        /// Lấy template nhập khẩu
        /// </summary>
        /// <returns></returns>
        Task<byte[]> DowloadTemplateImportDictionary();

        /// <summary>
        /// Xuất khẩu
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        Task<byte[]> ExportDictionary(string userId, string dictionaryId);

    }
}
