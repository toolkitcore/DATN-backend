using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IDictionaryRepository _dictionaryRepository;

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMemoryCache _memCache;
        private readonly ISessionService _sessionService;
        private readonly IMailService _mailService;

        private const string CallbackLinkActivateAccount = "http://localhost:3000/activate-account/";
        private const string CallbackLinkResetPassword = "http://localhost:3000/reset-password/";

        #endregion

        #region Constructor

        public AccountService(IUserRepository userRepository,
            IDictionaryRepository dictionaryRepository,
            IConfiguration configuration,
            IHttpContextAccessor httpContext,
            IMemoryCache memCache,
            IHustServiceCollection serviceCollection,
            ISessionService sessionService,
            IMailService mailService) : base(serviceCollection)
        {
            _userRepository = userRepository;
            _dictionaryRepository = dictionaryRepository;
            _configuration = configuration;
            _httpContext = httpContext;
            _memCache = memCache;
            _sessionService = sessionService;
            _mailService = mailService;
        }

        #endregion

        #region Method

        /// <summary>
        /// Hàm xử lý đăng ký tài khoản
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IServiceResult> Register(string userName, string password)
        {
            var res = new ServiceResult();

            // Kiểm tra thông tin đăng ký
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
        /// Hàm xử lý yêu cầu gửi email xác minh tài khoản
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IServiceResult> SendActivateEmail(string userName, string password)
        {
            var res = new ServiceResult();
            var keyThrottle = $"SendActivateEmail_{userName}";
            // Kiểm tra thời gian chặn api call liên tục
            var waitTime = GetThrottleTime(keyThrottle);
            if (waitTime > 0)
            {
                res.OnError(ErrorCode.TooManyRequests, ErrorMessage.TooManyRequests, data: waitTime);
                return res;
            }

            // Kiểm tra thông tin đăng nhập
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
                return res;
            }

            // Kiểm tra tài khoản: tài khoản phải tồn tại và ở trạng thái chưa kích hoạt
            var existUser = await _userRepository.SelectObject<User>(new Dictionary<string, object>()
            {
                { nameof(Models.Entity.user.user_name), userName },
                { nameof(Models.Entity.user.status), (int)UserStatus.NotActivated },
            }) as User;

            if (existUser == null || !SecurityUtil.VerifyPassword(password, existUser.Password))
            {
                res.OnError(ErrorCode.Err1002, ErrorMessage.Err1002);
                return res;
            }

            var tokenActivateAccount = this.GenerateTokenActivateAccount(existUser.UserId.ToString());
            await _mailService.SendEmailActivateAccount(userName, $"{CallbackLinkActivateAccount}{tokenActivateAccount}");

            // Nếu tất cả thực hiện thành công => set thời gian chặn call api liên tục
            SetThrottleTime(keyThrottle);

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

            // Đọc token
            var payload = this.ReadCallbackToken(token);
            if (payload == null || payload.TimeExpired < DateTime.Now || payload.Name != CallbackTokenName.ActivateAccount)
            {
                return res.OnError(ErrorCode.Err1003, ErrorMessage.Err1003);
            }

            // Tìm user
            var user = await _userRepository.SelectObject<User>(new Dictionary<string, object>()
            {
                { nameof(Models.Entity.user.user_id), payload.UserId }
            }) as User;

            if (user == null)
            {
                return res.OnError(ErrorCode.Err1003, ErrorMessage.Err1003);
            }

            if (user.Status == (int)UserStatus.Active)
            {
                return res.OnSuccess(message: Properties.Resources.ActivateAccount_AlreadyActivated);
            }

            // Kích hoạt tài khoản
            await _userRepository.CreateActivatedAccountData(user.UserId.ToString());

            return res.OnSuccess(message: Properties.Resources.ActivateAccount_Activated);
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

            var lastDictionary = lstDictionary
                .OrderByDescending(x => x.LastViewAt) // null sẽ xuống cuối
                .ThenByDescending(x => x.CreatedDate) // nếu cùng lastViewAt (vd: cùng null) => sx theo thời gian tạo mới nhất
                .FirstOrDefault();
            user.DictionaryId = lastDictionary.DictionaryId;

            // Cập nhật thời điểm xem dictionary
            var _ = await _dictionaryRepository.Update(new
            {
                dictionary_id = lastDictionary.DictionaryId,
                last_view_at = DateTime.Now
            });

            // Xóa token, session nếu có
            this.RemoveCurrentSession();
            // Sinh token, session
            var sessionId = this.GenerateSession(user);
            // Gán session vào response trả về
            this.SetResponseSessionCookie(sessionId);

            res.OnSuccess(new
            {
                SessionId = sessionId,
                user.UserId,
                user.UserName,
                user.DisplayName,
                user.Avatar,
                lastDictionary.DictionaryId,
                lastDictionary.DictionaryName
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

            this.RemoveCurrentSession();

            return Task.FromResult(res.OnSuccess());
        }

        /// <summary>
        /// Xử lý gửi email hệ thống chứa link reset mật khẩu tới email mà người dung cung cấp
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<IServiceResult> ForgotPassword(string email)
        {
            var res = new ServiceResult();
            var keyThrottle = $"ForgotPassword_{email}";
            // Kiểm tra thời gian chặn api call liên tục
            var waitTime = GetThrottleTime(keyThrottle);
            if (waitTime > 0)
            {
                res.OnError(ErrorCode.TooManyRequests, ErrorMessage.TooManyRequests, data: waitTime);
                return res;
            }

            // Kiểm tra thông tin email
            if (string.IsNullOrEmpty(email))
            {
                res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
                return res;
            }

            // Kiểm tra tài khoản phải tồn tại
            var existUser = await _userRepository.SelectObject<User>(new Dictionary<string, object>()
            {
                { nameof(Models.Entity.user.user_name), email }
            }) as User;

            if (existUser == null)
            {
                res.OnError(ErrorCode.Err1002, ErrorMessage.Err1002);
                return res;
            }

            var tokenResetPassword = this.GenerateTokenResetPassword(existUser.UserId.ToString());
            await _mailService.SendEmailResetPassword(email, $"{CallbackLinkResetPassword}{tokenResetPassword}");

            // Nếu tất cả thực hiện thành công => set thời gian chặn call api liên tục
            SetThrottleTime(keyThrottle);

            res.OnSuccess();
            return res;
        }

        /// <summary>
        /// Kiểm tra quyền truy cập trang reset mật khẩu
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<IServiceResult> CheckAccessResetPassword(string token)
        {
            var res = new ServiceResult();
            // Đọc token
            var payload = this.ReadCallbackToken(token);
            if (payload == null || payload.TimeExpired < DateTime.Now || payload.Name != CallbackTokenName.ResetPassword)
            {
                return Task.FromResult(res.OnError(ErrorCode.Err1003, ErrorMessage.Err1003));
            }

            return Task.FromResult(res.OnSuccess());
        }

        /// <summary>
        /// Xử lý Reset mật khẩu cho người dùng quên mật khẩu.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<IServiceResult> ResetPassword(string token, string newPassword)
        {
            var res = new ServiceResult();

            // Đọc token
            var payload = this.ReadCallbackToken(token);
            if (payload == null || payload.TimeExpired < DateTime.Now || payload.Name != CallbackTokenName.ResetPassword)
            {
                return res.OnError(ErrorCode.Err1003, ErrorMessage.Err1003);
            }

            // Tìm user
            var user = await _userRepository.SelectObject<User>(new Dictionary<string, object>()
            {
                { nameof(Models.Entity.user.user_id), payload.UserId }
            }) as User;

            if (user == null)
            {
                return res.OnError(ErrorCode.Err1003, ErrorMessage.Err1003);
            }

            // Cập nhật mật khẩu
            var paramUpdate = new
            {
                user_id = user.UserId,
                password = SecurityUtil.HashPassword(newPassword)
            };

            if (await _userRepository.Update(paramUpdate))
            {
                res.OnSuccess();
            }
            else
            {
                res.OnError(ErrorCode.Err9999);
            }

            return res;
        }

        /// <summary>
        /// Lấy thông tin tài khoản: user, dictionary
        /// </summary>
        /// <returns></returns>
        public async Task<IServiceResult> GetAccountInfo()
        {
            var res = new ServiceResult();

            var user = await _userRepository.SelectObject<User>(new
            {
                user_id = this.ServiceCollection.AuthUtil.GetCurrentUserId()
            }) as User;

            var dict = await _dictionaryRepository.SelectObject<Dictionary>(new
            {
                dictionary_id = this.ServiceCollection.AuthUtil.GetCurrentDictionaryId()
            }) as Dictionary;

            return res.OnSuccess(new
            {
                User = user,
                Dictionary = dict
            });
        }
        #endregion


        #region Helper

        /// <summary>
        /// Sinh session mới
        /// </summary>
        /// <param name="user"></param>
        public string GenerateSession(User user)
        {
            var sessionId = Guid.NewGuid().ToString();
            var token = SecurityUtil.GenerateToken(user, _configuration);
            _sessionService.SetToken(sessionId, token);
            return sessionId;
        }

        /// <summary>
        /// Xóa session cũ
        /// </summary>
        public void RemoveCurrentSession()
        {
            var reqHeader = _httpContext.HttpContext.Request.Headers; // chỉ cần check header vì middleware đã gán từ cookie lên header
            if (reqHeader.ContainsKey(AuthKey.SessionId))
            {
                var sessionId = reqHeader[AuthKey.SessionId];
                if (!string.IsNullOrEmpty(sessionId))
                {
                    _sessionService.RemoveToken(sessionId);
                }
            }
        }

        /// <summary>
        /// Set session cho request response hiện tại
        /// </summary>
        /// <param name="sessionId"></param>
        public void SetResponseSessionCookie(string sessionId)
        {
            _httpContext.HttpContext.Response.Cookies.Append(AuthKey.SessionId, sessionId, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(SecurityUtil.GetAuthTokenLifeTime(this._configuration)),
                HttpOnly = true
            });
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
                Name = CallbackTokenName.ActivateAccount,
                TimeExpired = DateTime.Now.AddDays(2) // Không dùng UtcNow, có thể lúc serialize bị convert
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
                Name = CallbackTokenName.ResetPassword,
                TimeExpired = DateTime.Now.AddMinutes(30) // Không dùng UtcNow
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
            if (string.IsNullOrEmpty(token))
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
            catch (Exception)
            {
                return null;
            }


        }

        /// <summary>
        /// Lấy key cache throttle hạn chế thời gian call api
        /// </summary>
        /// <returns></returns>
        public string GetThrottleCacheKey(string name)
        {
            var clientIp = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString();
            return $"{name}-{clientIp}";
        }

        /// <summary>
        /// Kiểm tra thời gian cần chờ trước khi call api liên tục
        /// </summary>
        /// <param name="name"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public double GetThrottleTime(string name)
        {
            var key = GetThrottleCacheKey(name);
            double waitTime = 0;

            var now = DateTime.Now;
            var timeExpired = _memCache.Get<DateTime?>(key);
            if (timeExpired != null)
            {
                waitTime = ((DateTime)timeExpired - now).TotalSeconds;
            }

            return waitTime;
        }

        /// <summary>
        /// Set thời gian chặn call api liên tục
        /// </summary>
        /// <param name="name"></param>
        /// <param name="seconds"></param>
        public void SetThrottleTime(string name, int seconds = 120)
        {
            var key = GetThrottleCacheKey(name);
            var timeExpired = DateTime.Now.AddSeconds(seconds);
            _memCache.Set(key,
                timeExpired,
                timeExpired
            );
        }

        #endregion

    }


}
