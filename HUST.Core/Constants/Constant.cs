using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Constants
{
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
    /// Tên file mặc định
    /// </summary>
    public static class FileDefaultName
    {
        // Có thể có hoặc không dùng đuôi .xlsx
        public const string DefaultTemplate = "default_template";
        public const string DefaultTemplateProtect = "default_template_protect";

        public const string DownloadDefaultTemplate = "HUST-PVO_ImportTemplate.xlsx";
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
    /// Tên sheet trong file mẫu nhập khẩu
    /// </summary>
    public static class ImportTemplateWorksheet
    {
        public const string Config = "Config";
        public const string Concept = "Concept";
        public const string Example = "Example";
        public const string ConceptRelationship = "Concept relationship";
        public const string ExampleRelationship = "Example relationship";
    }

    /// <summary>
    /// Tên sheet trong file mẫu nhập khẩu
    /// </summary>
    public static class ImportTemplateWorksheetStructure
    {
        public static class Config
        {
            public const string ConceptLinkCellAddress = "B3";
            public const string ExampleLinkCellAddress = "D3";
            public const string ToneCellAddress = "F3";
            public const string ModeCellAddress = "H3";
            public const string RegisterCellAddress = "J3";
            public const string NuanceCellAddress = "L3";
            public const string DialectCellAddress = "N3";
        }
        public static class Concept
        {

        }

        public static class Example
        {

        }

        public static class ConceptRelationship
        {

        }

        public static class ExampleRelationship
        {

        }
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
