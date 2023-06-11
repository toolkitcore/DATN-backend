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
    /// Lớp controller cung cấp api helper
    /// </summary>
    public class HelperController : BaseApiController
    {
        #region Fields
        private readonly IExternalApiService _service;
        #endregion

        #region Constructors

        public HelperController(IHustServiceCollection serviceCollection,
            IExternalApiService service) : base(serviceCollection)
        {
            _service = service;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Lấy dữ liệu wordsapi
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        [HttpGet("wordsapi")]
        public async Task<IServiceResult> GetWordsapiResult(string word)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.GetWordsapiResult(word);
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
