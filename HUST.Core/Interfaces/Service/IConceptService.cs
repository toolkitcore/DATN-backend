using HUST.Core.Models.DTO;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Service
{
    /// <summary>
    /// Service xử lý concept
    /// </summary>
    public interface IConceptService
    {
        /// <summary>
        /// Lấy danh sách concept trong từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        Task<IServiceResult> GetListConcept(string dictionaryId);

        /// <summary>
        /// Thêm 1 concept vào từ điển
        /// </summary>
        /// <param name="concept"></param>
        /// <returns></returns>
        Task<IServiceResult> AddConcept(Concept concept);

        /// <summary>
        /// Thực hiện cập nhật tên, mô tả của concept
        /// </summary>
        /// <param name="concept"></param>
        /// <returns></returns>
        Task<IServiceResult> UpdateConcept(Concept concept);

        /// <summary>
        /// Thực hiện xóa concept
        /// </summary>
        /// <param name="conceptId"></param>
        /// <param name="isForced"></param>
        /// <returns></returns>
        Task<IServiceResult> DeleteConcept(string conceptId, bool? isForced);

        /// <summary>
        /// Lấy dữ liệu concept và các example liên kết với concept đó
        /// </summary>
        /// <param name="conceptId"></param>
        /// <returns></returns>
        Task<IServiceResult> GetConcept(string conceptId);

        /// <summary>
        /// Lấy danh sách concept trong từ điển mà khớp với xâu tìm kiếm của người dùng
        /// </summary>
        /// <param name="searchKey"></param>
        /// <param name="dictionaryId"></param>
        /// <param name="isSearchSoundex"></param>
        /// <returns></returns>
        Task<IServiceResult> SearchConcept(string searchKey, string dictionaryId, bool? isSearchSoundex);

        /// <summary>
        /// Lấy ra mối quan hệ liên kết giữa concept con và concept cha.
        /// </summary>
        /// <param name="conceptId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        Task<IServiceResult> GetConceptRelationship(string conceptId, string parentId);

        /// <summary>
        /// Thực hiện cập nhật (hoặc tạo mới nếu chưa có) liên kết giữa 2 concept
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IServiceResult> UpdateConceptRelationship(UpdateConceptRelationshipParam param);

        /// <summary>
        /// Thực hiện lấy danh sách gợi ý concept từ những từ khóa người dùng cung cấp.
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        Task<ServiceResult> GetListRecommendConcept(List<string> keywords, Guid? dictionaryId);
    }
}
