using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Service;
using HUST.Core.Interfaces.Util;
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
    /// Lớp controller cung cấp api liên quan đến dashboard
    /// </summary>
    public class DashboardController : BaseApiController
    {
        #region Fields
        private readonly IConceptService _conceptService;
        private readonly IExampleService _exampleService;
        #endregion

        #region Constructors

        public DashboardController(IHustServiceCollection serviceCollection, 
            IConceptService conceptService,
            IExampleService exampleService) : base(serviceCollection)
        {
            _conceptService = conceptService;
            _exampleService = exampleService;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Thực hiện lấy danh sách top concept thêm mới gần đây nhất
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("get_list_most_recent_concept")]
        public async Task<IServiceResult> GetListMostRecentConcept([FromQuery]int limit)
        {
            var res = new ServiceResult();
            try
            {
                return await _conceptService.GetListMostRecentConcept(limit);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Thực hiện lấy danh sách top example thêm mới gần đây nhất
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("get_list_most_recent_example")]
        public async Task<IServiceResult> GetListMostRecentExample([FromQuery] int limit)
        {
            var res = new ServiceResult();
            try
            {
                return await _exampleService.GetListMostRecentExample(limit);
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
