using HUST.Core.Models.DTO;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Backup dữ liệu và gửi vào email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        Task<IServiceResult> BackupData(string email, string dictionaryId);

        /// <summary>
        /// Lấy tên file export
        /// </summary>
        /// <param name="dictionaryName"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        string GetExportFileName(string dictionaryName, DateTime? dateTime = null);

        /// <summary>
        /// Nhập khẩu và validate (bước đầu)
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<IServiceResult> ImportDictionary(string dictionaryId, IFormFile file);

        /// <summary>
        /// Lưu dữ liệu nhập khẩu
        /// </summary>
        /// <param name="importSession"></param>
        /// <returns></returns>
        Task<IServiceResult> DoImport(string importSession);
    }
}
