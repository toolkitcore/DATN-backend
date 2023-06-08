using HUST.Core.Models.DTO;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Service
{
    /// <summary>
    /// Service xử lý dữ liệu dictionary
    /// </summary>
    public interface IDictionaryService
    {
        /// <summary>
        /// Lấy thông tin từ điển theo id
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        Task<IServiceResult> GetDictionaryById(string dictionaryId);

        /// <summary>
        /// Lấy danh sách từ điển đã tạo của người dùng
        /// </summary>
        /// <returns></returns>
        Task<IServiceResult> GetListDictionary();

        /// <summary>
        /// Truy cập vào từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        Task<IServiceResult> LoadDictionary(string dictionaryId);

        /// <summary>
        /// Thêm 1 từ điển mới 
        /// </summary>
        /// <param name="dictionaryName"></param>
        /// <param name="cloneDictionaryId"></param>
        /// <returns></returns>
        Task<IServiceResult> AddDictionary(string dictionaryName, string cloneDictionaryId);

        /// <summary>
        /// Thực hiện cập nhật tên từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <param name="dictionaryName"></param>
        /// <returns></returns>
        Task<IServiceResult> UpdateDictionary(string dictionaryId, string dictionaryName);

        /// <summary>
        /// Thực hiện xóa từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        Task<IServiceResult> DeleteDictionary(string dictionaryId);

        /// <summary>
        /// Thực hiện xóa dữ liệu từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        Task<IServiceResult> DeleteDictionaryData(string dictionaryId);

        /// <summary>
        /// Thực hiện copy dữ liệu từ từ điển nguồn và gộp vào dữ liệu ở từ điển đích
        /// </summary>
        /// <param name="sourceDictionaryId"></param>
        /// <param name="destDictionaryId"></param>
        /// <param name="isDeleteData"></param>
        /// <returns></returns>
        Task<IServiceResult> TransferDictionary(string sourceDictionaryId, string destDictionaryId, bool? isDeleteData);

        /// <summary>
        /// Lấy số lượng concept, example trong 1 từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        Task<IServiceResult> GetNumberRecord(Guid dictionaryId);
    }
}
