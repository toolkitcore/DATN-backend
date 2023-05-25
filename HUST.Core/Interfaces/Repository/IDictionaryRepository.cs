using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using System;
using System.Data;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Repository
{

    public interface IDictionaryRepository: IBaseRepository<dictionary>
    {
        /// <summary>
        /// Thực hiện clone dữ liệu từ điển (có xóa dữ liệu từ điển đích)
        /// </summary>
        /// <param name="sourceDictionaryId"></param>
        /// <param name="destDictionaryId"></param>
        /// <returns></returns>
        Task<bool> CloneDictionaryData(Guid sourceDictionaryId, Guid destDictionaryId, IDbTransaction transaction = null);
    }
}
