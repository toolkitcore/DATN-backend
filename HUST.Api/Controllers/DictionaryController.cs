using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HUST.Api.Controllers
{
    public class DictionaryController : BaseApiController
    {
        #region Fields
        private readonly IDictionaryService _service;
        #endregion

        #region Constructor
        public DictionaryController(IHustServiceCollection serviceCollection, IDictionaryService service) : base(serviceCollection)
        {
            _service = service;
        }
        #endregion

        /// <summary>
        /// Lấy danh sách từ điển đã tạo của người dùng
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_list_dictionary")]
        public async Task<IServiceResult> GetListDictionary()
        {
            var res = new ServiceResult();
            try
            {
                return await _service.GetListDictionary();
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Truy cập vào từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        [HttpGet("load_dictionary")]
        public async Task<IServiceResult> LoadDictionary([FromQuery] string dictionaryId)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.LoadDictionary(dictionaryId);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }
    }
}
