using HUST.Core.Models.DTO;
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
        /// <returns></returns>
        Task<IServiceResult> DeleteConcept(string conceptId);

        Task<ServiceResult> GetListRecommendConcept(List<string> keywords, Guid dictionaryId);
    }
}
