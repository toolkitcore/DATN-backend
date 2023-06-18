using HUST.Core.Enums;
using System;

namespace HUST.Core.Models.ServerObject
{
    /// <summary>
    /// Token truyền vào callback trong email
    /// </summary>
    public class CallbackTokenPayload
    {
        public string UserId { get; set; }
        public string Name { get; set; }

        public DateTime? TimeExpired { get; set; }
    }
}
