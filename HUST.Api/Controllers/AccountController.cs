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
    /// Lớp controller cung cấp api liên quan đến tài khoản
    /// </summary>
    public class AccountController : BaseApiController
    {
        #region Fields
        private readonly IAccountService _service;
        #endregion

        #region Constructors

        public AccountController(IHustServiceCollection serviceCollection, IAccountService service) : base(serviceCollection)
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
        [HttpPost("login"), AllowAnonymous]
        public async Task<IServiceResult> Login([FromBody]LoginParam param)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.Login(param.UserName, param.Password);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }
            
            return res;
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        [HttpGet("logout")]
        public async Task<IServiceResult> Logout()
        {
            var res = new ServiceResult();
            try
            {
                return await _service.Logout();
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("register"), AllowAnonymous]
        public async Task<IServiceResult> Register([FromBody] RegisterParam param)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.Register(param.UserName, param.Password);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Kích hoạt tài khoản
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet("activate_account"), AllowAnonymous]
        public async Task<IServiceResult> ActivateAccount(string token)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.ActivateAccount(token);
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
