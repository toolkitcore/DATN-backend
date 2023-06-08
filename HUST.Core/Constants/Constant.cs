using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Constants
{
    /// <summary>
    /// Extension của file
    /// </summary>
    public static class FileExtension
    {
        public const string Excel2007 = ".xlsx";
        public static readonly string[] Image = {".png", ".jpeg", ".jpg"};
    }

    /// <summary>
    /// Loại dữ liệu của file
    /// </summary>
    public static class FileContentType
    {
        public const string OctetStream = "application/octet-stream";
        public const string Excel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public static readonly string[] Image =
        {
            "image/png", 
            "image/jpeg"
        };
    }

    /// <summary>
    /// Đường dẫn folder storage
    /// </summary>
    public static class StoragePath
    {
        public const string Avatar = "avatar";
        public const string Import = "import";
    }

    /// <summary>
    /// Đường dẫn folder server
    /// </summary>
    public static class ServerStoragePath
    {
        public const string Import = "File/Import";
    }

    /// <summary>
    /// Giá trị xác định trước độ mạnh của liên kết
    /// </summary>
    public static class LinkStrength
    {
        public const int TwoPrimary = 2;
        public const int PrimaryAndAssociatedNonPrimary = 2;
        public const int TwoAssociatedNonPrimary = 1;
        public const int PrimaryAndUnAssociatedNonPrimary = 0;
        public const int TwoUnassociatedNonPrimary = -1;
        public const int Itself = 1;
    }

}
