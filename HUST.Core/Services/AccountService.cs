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
        #region Field

        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ISessionService _sessionService;
        private readonly IMailService _mailService;

        private const string CallbackLinkActivateAccount = "http://localhost:3000/activate-account/";
        private const string CallbackLinkResetPassword = "http://localhost:3000/reset-password/";

        #endregion

        #region Constructor

        public AccountService(IUserRepository userRepository,
            IConfiguration configuration,
            IHttpContextAccessor httpContext,
            IHustServiceCollection serviceCollection,
            ISessionService sessionService,
            IMailService mailService) : base(serviceCollection)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _httpContext = httpContext;
            _sessionService = sessionService;
            _mailService = mailService;
        }

        #endregion

        #region Method
        /// <summary>
        /// Hàm xử lý login
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IServiceResult> Login(string userName, string password)
        {
            var res = new ServiceResult();

            // Kiểm tra thông tin đăng nhập
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                res.OnError(ErrorCode.Err1000, ErrorMessage.Err1000);
                return res;
            }

            // Lấy user từ db để kiểm tra
            var user = await _userRepository.SelectObject<User>(new Dictionary<string, object>()
            {
                { nameof(Models.Entity.user.user_name), userName }
            }) as User;
            if (user == null || !SecurityUtil.VerifyPassword(password, user.Password))
            {
                res.OnError(ErrorCode.Err1000, ErrorMessage.Err1000);
                return res;
            }

            // Kiểm tra trạng thái tài khoản
            if (user.Status == (int)UserStatus.NotActivated)
            {
                res.OnError(ErrorCode.Err1004, ErrorMessage.Err1004);
                return res;
            }

            // Lấy thông tin dictionary dùng gần nhất
            var lstDictionary = await _userRepository.SelectObjects<Dictionary>(new Dictionary<string, object>()
            {
                { nameof(Models.Entity.dictionary.user_id), user.UserId }
            });
            var lastDictionary = lstDictionary?.OrderByDescending(x => x.LastViewAt)?.FirstOrDefault();
            user.DictionaryId = lastDictionary?.DictionaryId;

            // Sinh token, session
            var sessionId = GenerateSession(user);
            _httpContext.HttpContext.Response.Cookies.Append(AuthKey.SessionId, sessionId, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(SecurityUtil.GetAuthTokenLifeTime(this._configuration)),
                HttpOnly = true
            });


            res.OnSuccess(new
            {
                SessionId = sessionId,
                user.UserId,
                user.UserName,
                user.DisplayName,
                user.Avatar,
                lastDictionary?.DictionaryId,
                lastDictionary?.DictionaryName
            });

            return res;
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        public Task<IServiceResult> Logout()
        {
            var res = new ServiceResult();
            var reqHeader = _httpContext.HttpContext.Request.Headers;
            if (reqHeader.ContainsKey(AuthKey.SessionId))
            {
                var sessionId = reqHeader[AuthKey.SessionId];
                if (!string.IsNullOrEmpty(sessionId))
                {
                    _sessionService.RemoveToken(sessionId);
                }
            }

            return Task.FromResult(res.OnSuccess());
        }

        /// <summary>
        /// Hàm xử lý đăng ký tài khoản
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IServiceResult> Register(string userName, string password)
        {
            var res = new ServiceResult();

            // Kiểm tra thông tin đăng nhập
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
                return res;
            }

            // Kiểm tra tên đăng nhập (email) đã được sử dụng chưa
            var existUser = await _userRepository.SelectObject<User>(new Dictionary<string, object>()
            {
                { nameof(Models.Entity.user.user_name), userName }
            }) as User;
            if (existUser != null)
            {
                res.OnError(ErrorCode.Err1001, ErrorMessage.Err1001);
                return res;
            }

            // Insert user vào db
            var user = new Models.Entity.user
            {
                user_id = Guid.NewGuid(),
                user_name = userName,
                email = userName,
                password = SecurityUtil.HashPassword(password),
                created_date = DateTime.Now,
                status = (int)UserStatus.NotActivated
            };

            await _userRepository.Insert(user);

            var tokenActivateAccount = this.GenerateTokenActivateAccount(user.user_id.ToString());
            await _mailService.SendEmailActivateAccount(userName, $"{CallbackLinkActivateAccount}{tokenActivateAccount}");

            res.OnSuccess();
            return res;
        }

        /// <summary>
        /// Hàm xử lý kích hoạt tài khoản
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IServiceResult> ActivateAccount(string token)
        {
            var res = new ServiceResult();

            var payload = this.ReadCallbackToken(token);
            if(payload == null || payload.TimeExpired < DateTime.UtcNow)
            {
                return res.OnError(ErrorCode.Err1003, Properties.Resources.ActivateAccount_CouldNotFind);
            }

            var user = await _userRepository.SelectObject<User>(new Dictionary<string, object>()
            {
                { nameof(Models.Entity.user.user_id), payload.UserId }
            }) as User;

            if (user == null)
            {
                return res.OnError(ErrorCode.Err1003, Properties.Resources.ActivateAccount_CouldNotFind);
            }

            if (user.Status == (int)UserStatus.Active)
            {
                return res.OnSuccess(message: Properties.Resources.ActivateAccount_AlreadyActivated);
            }

            //var paramUpdate = new
            //{
            //    user_id = user.UserId,
            //    status = (int)UserStatus.Active
            //};

            //if(await _userRepository.CreateActivatedAccountData)
            //{
            //    res.OnSuccess(message: Properties.Resources.ActivateAccount_Activated);
            //} else
            //{
            //    res.OnError(ErrorCode.Err9999);
            //}

            await _userRepository.CreateActivatedAccountData(user.UserId.ToString());

            return res.OnSuccess(message: Properties.Resources.ActivateAccount_Activated);
        }
        #endregion


        #region Helper

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

        /// <summary>
        /// Sinh token kích hoạt tài khoản 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GenerateTokenActivateAccount(string userId)
        {
            //var key = Guid.NewGuid().ToString();
            //this.ServiceCollection.CacheUtil.SetStringAsync(key, userId, TimeSpan.FromDays(2));
            //return key;

            var payload = new CallbackTokenPayload
            {
                UserId = userId,
                TimeExpired = DateTime.UtcNow.AddDays(2)
            };

            var cypherText = SecurityUtil.EncryptString(SerializeUtil.SerializeObject(payload), configuration: _configuration);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(cypherText));
        }


        /// <summary>
        /// Sinh token reset mật khẩu
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GenerateTokenResetPassword(string userId)
        {
            //var key = Guid.NewGuid().ToString();
            //this.ServiceCollection.CacheUtil.SetStringAsync(key, userId, TimeSpan.FromMinutes(30));
            //return key;

            var payload = new CallbackTokenPayload
            {
                UserId = userId,
                TimeExpired = DateTime.UtcNow.AddMinutes(30)
            };

            var cypherText = SecurityUtil.EncryptString(SerializeUtil.SerializeObject(payload), configuration: _configuration);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(cypherText));
        }

        /// <summary>
        /// Đọc token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private CallbackTokenPayload ReadCallbackToken(string token)
        {
            if(string.IsNullOrEmpty(token))
            {
                return null;
            }

            try
            {
                byte[] data = Convert.FromBase64String(token);
                var decodeText = Encoding.UTF8.GetString(data);
                var plainText = SecurityUtil.DecryptString(decodeText, configuration: _configuration);
                if (string.IsNullOrEmpty(plainText))
                {
                    return null;
                }
                var payload = SerializeUtil.DeserializeObject<CallbackTokenPayload>(plainText);
                return payload;
            }
            catch (Exception ex)
            {
                return null;
            }
            

        }
        #endregion

    }


}
