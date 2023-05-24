using HUST.Core.Constants;
using HUST.Core.Models.DTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Utils
{
    /// <summary>
    /// Các hàm util liên quan đến authen
    /// </summary>
    public class AuthUtil : IAuthUtil
    {
        #region Declaration

        private readonly IHttpContextAccessor _httpContext;
        private User _user;

        #endregion

        #region Properties

        public User User => _user ??= GetCurrentUser();

        #endregion

        #region Constructor

        public AuthUtil(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Lấy ra đối tượng user đang đăng nhập
        /// </summary>
        /// <returns></returns>
        public User GetCurrentUser()
        {
            var user = new User();
            if(_httpContext.HttpContext != null)
            {
                var jsonPayload = GetAuthPayloadString();
                if(!string.IsNullOrEmpty(jsonPayload))
                {
                    user = SerializeUtil.DeserializeObject<User>(jsonPayload);
                }
            }

            return user;
        }

        /// <summary>
        /// Lấy ra chuỗi thông tin auth từ payload của token
        /// </summary>
        /// <returns></returns>
        private string GetAuthPayloadString()
        {
            var authHeader = GetHeaderByName(AuthKey.Authorization);
            if(!string.IsNullOrEmpty(authHeader))
            {
                string token = authHeader.Split(' ')[1];
                if(!string.IsNullOrEmpty(token))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadJwtToken(token);
                    return jsonToken.Payload.SerializeToJson();
                } 
                else
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Lấy giá trị Header thông qua tên key
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        private string GetHeaderByName(string keyName)
        {
            return _httpContext?.HttpContext?.Request?.Headers[keyName] + "";
        }

        /// <summary>
        /// Lấy id user đang đăng nhập
        /// </summary>
        /// <returns></returns>
        public Guid? GetCurrentUserId()
        {
            return this.User.UserId;
        }

        /// <summary>
        /// Lấy id dictionary đang sử dụng
        /// </summary>
        /// <returns></returns>
        public Guid? GetCurrentDictionaryId()
        {
            return this.User.DictionaryId;
        }

        #endregion


    }
}
