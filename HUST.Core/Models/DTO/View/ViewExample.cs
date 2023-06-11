using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.DTO
{
    public class ViewExample
    {
        public Guid? DictionaryId { get; set; }
        public Guid? ExampleId { get; set; }
        public string Detail { get; set; }
        public string DetailHtml { get; set; }
        public string ToneName { get; set; }
        public string ModeName { get; set; }
        public string RegisterName { get; set; }
        public string NuanceName { get; set; }
        public string DialectName { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
