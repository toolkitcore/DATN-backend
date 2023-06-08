using Dapper.Contrib.Extensions;
using OfficeOpenXml.Attributes;
using System;

namespace HUST.Core.Models.ServerObject
{
    /// <summary>
    /// Đối tượng xuất/nhập khẩu: map với view_example_relationship
    /// </summary>
    public class ExampleRelationshipImport
    {
        [EpplusTableColumn(Order = 1)]
        public string ExampleHtml { get; set; }

        [EpplusTableColumn(Order = 2)]
        public string Concept { get; set; }

        [EpplusTableColumn(Order = 3)]
        public string ExampleLinkName { get; set; }
    }
}
