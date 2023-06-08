using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Constants
{
    public class TemplateConfig
    {
        /// <summary>
        /// Tên file mặc định
        /// </summary>
        public static class FileDefaultName
        {
            // Có thể có hoặc không dùng đuôi .xlsx
            public const string DefaultTemplate = "default_template";
            public const string DefaultTemplateProtect = "default_template_protect";

            public const string DownloadDefaultTemplate = "HUSTPVO_ImportTemplate.xlsx";
            public const string ExportFile = "HUSTPVO_{0}_{1}.xlsx";
        }

        /// <summary>
        /// Thuộc tính custom
        /// </summary>
        public static class CustomProperty
        {
            public const string TokenPropertyName = "Token";
            public const string TokenPropertyValue = "b9ef3f10-0156-11ee-aac4-a44cc8756a37";
        }

        /// <summary>
        /// Tên sheet trong file mẫu nhập khẩu
        /// </summary>
        public static class WorksheetName
        {
            public const string Config = "Config";
            public const string Concept = "Concept";
            public const string Example = "Example";
            public const string ConceptRelationship = "Concept relationship";
            public const string ExampleRelationship = "Example relationship";
        }

        /// <summary>
        /// Dòng bắt đầu dữ liệu
        /// </summary>
        public const int StartRowData = 2;

        /// <summary>
        /// Cột bắt đầu dữ liệu
        /// </summary>
        public const int StartColData = 2;

        /// <summary>
        /// Cấu hình sheet config
        /// </summary>
        public static class ConfigSheet
        {
            public const int ConceptLink = 2;
            public const int ExampleLink = 4;
            public const int Tone = 6;
            public const int Mode = 8;
            public const int Register = 10;
            public const int Nuance = 12;
            public const int Dialect = 14;
        }

        /// <summary>
        /// Cấu hình sheet concept
        /// </summary>
        public static class ConceptSheet
        {
            public const int Title = 2; // A
            public const int Description = 3; // B
            public const int Error = 4; // C
        }

        /// <summary>
        /// Cấu hình sheet example
        /// </summary>
        public static class ExampleSheet
        {
            public const int Example = 2;
            public const int Tone = 3;
            public const int Mode = 4;
            public const int Register = 5;
            public const int Nuance = 6;
            public const int Dialect = 7;
            public const int Note = 8;
            public const int Error = 9;
        }

        /// <summary>
        /// Cấu hình sheet Concept relationship
        /// </summary>
        public static class ConceptRelationshipSheet
        {
            public const int ChildConcept = 2;
            public const int ParentConcept = 3;
            public const int Relation = 4;
            public const int Error = 5;
        }

        /// <summary>
        /// Cấu hình sheet Example relationship
        /// </summary>
        public static class ExampleRelationshipSheet
        {
            public const int Example = 2;
            public const int Concept = 3;
            public const int Relation = 4;
            public const int Error = 5;
        }
    }
}
