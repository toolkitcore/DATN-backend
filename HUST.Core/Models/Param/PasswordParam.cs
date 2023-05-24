using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Models.Param
{
    /// <summary>
    /// Param xử lý reset mật khẩu
    /// </summary>
    public class PasswordParam
    {
        public string Token{ get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
