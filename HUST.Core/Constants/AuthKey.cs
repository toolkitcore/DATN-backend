using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Constants
{
    /// <summary>
    /// Key auth
    /// </summary>
    public static class AuthKey
    {
        public const string Authorization = "Authorization";
        public const string Bearer = "Bearer";
        public const string TokenExpired = "Token-Expired";
        public const string SessionId = "x-session-id";
        public const string HelperAppApiKey = "Hust-Helper-App-Key";
    }

    /// <summary>
    /// Key jwt claim
    /// </summary>
    public static class JwtClaimKey
    {
        public const string UserId = "UserId";
        public const string UserName = "UserName";
        public const string Email = "Email";
        public const string DictionaryId = "DictionaryId";
        public const string Status = "Status";
    }

}
