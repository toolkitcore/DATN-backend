using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Authorization;
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
        public readonly IHustServiceCollection ServiceCollection;
        public readonly IConfiguration Configuration;

        #endregion

        #region Constructors

        public AccountController(IHustServiceCollection serviceCollection, IConfiguration configuration)
        {
            ServiceCollection = serviceCollection;
            Configuration = configuration;
        }

        #endregion

        #region Methods

        [HttpPost("Login"), AllowAnonymous]
        public async Task<IServiceResult> Login([FromBody]User user)
        {
            var res = new ServiceResult();
            try
            {
                var fakeHash = SecurityUtil.HashPassword("12345678");

                if (user.UserName == "hieu.pt183535@gmail.com" && SecurityUtil.VerifyPassword(user.Password, fakeHash))
                {
                    var token = SecurityUtil.GenerateToken(user, Configuration);
                    res.OnSuccess(token);
                    await this.ServiceCollection.CacheUtil.SetStringAsync($"AccessToken_{user.UserName}", token);
                }
                else
                {
                    res.OnError(ErrorCode.LoginErrorCode.WrongLoginInfo, "Email or password are incorrect.");
                }

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
