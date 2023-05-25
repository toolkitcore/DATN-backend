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

        /// <summary>
        /// Thực hiện thêm 1 từ điển mới (có thể kèm việc copy dữ liệu từ 1 từ điển khác đã có)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("add_dictionary")]
        public async Task<IServiceResult> AddDictionary([FromBody] AddDictionaryParam param)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.AddDictionary(param.DictionaryName, param.CloneDictionaryId);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Thực hiện cập nhật tên từ điển
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPatch("update_dictionary")]
        public async Task<IServiceResult> UpdateDictionary([FromBody] Core.Models.DTO.Dictionary param)
        {
            // param.DictionaryId null thì sẽ có exception của .NET => lỗi 400


            var res = new ServiceResult();
            try
            {
                return await _service.UpdateDictionary(param.DictionaryId.ToString(), param.DictionaryName);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Thực hiện xóa từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        [HttpDelete("delete_dictionary")]
        public async Task<IServiceResult> DeleteDictionary([FromQuery] string dictionaryId)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.DeleteDictionary(dictionaryId);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }
    }
}
