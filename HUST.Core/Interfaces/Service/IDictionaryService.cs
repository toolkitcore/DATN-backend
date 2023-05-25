using HUST.Core.Models.DTO;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Service
{
    /// <summary>
    /// Service xử lý dữ liệu dictionary
    /// </summary>
    public interface IDictionaryService
    {
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
    }
}
