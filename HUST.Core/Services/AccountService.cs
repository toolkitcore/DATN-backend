using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Services
{
    /// <summary>
    /// Serivce xử lý account
    /// </summary>
    public class AccountService : BaseService, IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ISessionService _sessionService;


        public AccountService(IUserRepository userRepository, 
            IConfiguration configuration, 
            IHttpContextAccessor httpContext,
            IHustServiceCollection serviceCollection, 
            ISessionService sessionService) : base(serviceCollection)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _httpContext = httpContext;
            _sessionService = sessionService;
        }

        /// <summary>
        /// Hàm xử lý login
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IServiceResult> Login(string userName, string password)
        {
            var res = new ServiceResult();

            if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                res.OnError(ErrorCode.Err1000, null, ErrorMessage.Err1000);
                return res;
            }

            var param = new Dictionary<string, object>()
            {
                { nameof(Models.Entity.user.user_name), userName }
            };

            var user = await _userRepository.SelectObject<User>(param);
            if(user == null || !SecurityUtil.VerifyPassword(password, user.Password))
            {
                res.OnError(ErrorCode.Err1000, null, ErrorMessage.Err1000);
                return res;
            }

            if(user.Status == (int)UserStatus.NotActivated)
            {
                res.OnError(ErrorCode.Err1004, null, ErrorMessage.Err1004);
                return res;
            }

            var lstDictionary = await _userRepository.SelectObjects<Dictionary>(new Dictionary<string, object>()
            {
                { nameof(Models.Entity.dictionary.user_id), user.UserId }
            });
            var lastDictionary = lstDictionary?.OrderByDescending(x => x.LastViewAt)?.FirstOrDefault();

            user.DictionaryId = lastDictionary?.DictionaryId;
            var sessionId = GenerateSession(user);
            _httpContext.HttpContext.Response.Cookies.Append(AuthKey.SessionId, sessionId, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(14),
                HttpOnly = true
            });

            res.OnSuccess(new
            {
                SessionId = sessionId,
                UserId = user.UserId,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                Avatar = user.Avatar,
                DictionaryId = lastDictionary?.DictionaryId,
                DictionaryName = lastDictionary?.DictionaryName
            });

            return res;
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        public async Task<IServiceResult> Logout()
        {
            var res = new ServiceResult();
            var reqHeader = _httpContext.HttpContext.Request.Headers;
            if (reqHeader.ContainsKey(AuthKey.SessionId))
            {
                var sessionId = reqHeader[AuthKey.SessionId];
                if(!string.IsNullOrEmpty(sessionId))
                {
                    _sessionService.RemoveToken(sessionId);
                }
            }

            return res.OnSuccess(null);
        }

        /// <summary>
        /// Sinh session mới
        /// </summary>
        /// <param name="user"></param>
        private string GenerateSession(User user)
        {
            var sessionId = Guid.NewGuid().ToString();
            var token = SecurityUtil.GenerateToken(user, _configuration);
            _sessionService.SetToken(sessionId, token);
            return sessionId;
        }


    }


}
