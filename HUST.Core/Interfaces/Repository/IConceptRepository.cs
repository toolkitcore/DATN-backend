using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Repository
{

    public interface IConceptRepository: IBaseRepository<concept>
    {
        /// <summary>
        /// Thực hiện lấy danh sách concept trong từ điển mà khớp với xâu tìm kiếm của người dùng
        /// </summary>
        /// <param name="searchKey"></param>
        /// <param name="dictionaryId"></param>
        /// <param name="isSearchSoundex"></param>
        /// <returns></returns>
        Task<List<Concept>> SearchConcept(string searchKey, string dictionaryId, bool? isSearchSoundex);

        /// <summary>
        /// Thực hiện lấy danh sách top concept thêm mới gần đây nhất
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<List<Concept>> GetListMostRecentConcept(string dictionaryId, int limit);
    }
}
