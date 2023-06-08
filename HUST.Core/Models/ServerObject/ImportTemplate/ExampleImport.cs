using Dapper.Contrib.Extensions;
using OfficeOpenXml.Attributes;
using System;

namespace HUST.Core.Models.ServerObject
{
    /// <summary>
    /// Đối tượng xuất/nhập khẩu: map với view_example
    /// </summary>
    public class ExampleImport
    {
        [EpplusTableColumn(Order = 1)]
        public string DetailHtml { get; set; }

        [EpplusTableColumn(Order = 2)]
        public string ToneName { get; set; }

        [EpplusTableColumn(Order = 3)]
        public string ModeName { get; set; }

        [EpplusTableColumn(Order = 4)]
        public string RegisterName { get; set; }

        [EpplusTableColumn(Order = 5)]
        public string NuanceName { get; set; }

        [EpplusTableColumn(Order = 6)]
        public string DialectName { get; set; }

        [EpplusTableColumn(Order = 7)]
        public string Note { get; set; }
    }
}
