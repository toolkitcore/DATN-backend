using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Models.Param
{
    /// <summary>
    /// Param xử lý cập nhật thông tin người dùng
    /// </summary>
    public class UpdateUserInfoParam
    {
        public IFormFile Avatar { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string Birthday { get; set; }
        public string Position { get; set; }
    }
}
