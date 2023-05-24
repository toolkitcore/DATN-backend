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
