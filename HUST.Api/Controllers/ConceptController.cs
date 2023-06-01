using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HUST.Api.Controllers
{
    /// <summary>
    /// Lớp controller cung cấp api liên quan đến concept
    /// </summary>
    public class ConceptController : BaseApiController
    {
        #region Fields
        private readonly IConceptService _service;
        #endregion

        #region Constructors

        public ConceptController(IHustServiceCollection serviceCollection, IConceptService service) : base(serviceCollection)
        {
            _service = service;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Lấy danh sách concept trong từ điển
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_list_concept")]
        public async Task<IServiceResult> GetListConcept([FromQuery] string dictionaryId)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.GetListConcept(dictionaryId);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Thực hiện thêm 1 concept vào từ điển
        /// </summary>
        /// <param name="concept"></param>
        /// <returns></returns>
        [HttpPost("add_concept")]
        public async Task<IServiceResult> AddConcept([FromBody] Concept concept)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.AddConcept(concept);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }
        /// <summary>
        /// Thực hiện cập nhật tên, mô tả của concept
        /// </summary>
        /// <param name="concept"></param>
        /// <returns></returns>
        [HttpPut("update_concept")]
        public async Task<IServiceResult> UpdateConcept([FromBody] Concept concept)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.UpdateConcept(concept);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Thực hiện xóa concept
        /// </summary>
        /// <param name="conceptId"></param>
        /// <returns></returns>
        [HttpDelete("delete_concept")]
        public async Task<IServiceResult> DeleteConcept([FromQuery] string conceptId)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.DeleteConcept(conceptId);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }


        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet("get_list_recommend_concept"), AllowAnonymous]
        public async Task<IServiceResult> GetListRecommendConcept([FromQuery] List<string> keywords, [FromQuery] Guid dictionaryId)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.GetListRecommendConcept(keywords, dictionaryId);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }
            
            return res;
        }
        #endregion
    }
}
