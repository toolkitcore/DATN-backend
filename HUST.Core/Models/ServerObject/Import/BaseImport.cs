using Dapper.Contrib.Extensions;
using HUST.Core.Constants;
using OfficeOpenXml.Attributes;
using System;

namespace HUST.Core.Models.ServerObject
{
    /// <summary>
    /// Đối tượng xuất/nhập khẩu
    /// </summary>
    public class BaseImport
    {
        /// <summary>
        /// Index của dòng (bắt đầu từ 1, tương đương với chỉ số dòng excel)
        /// </summary>
        [EpplusIgnore]
        public int RowIndex { get; set; }
    }
}
