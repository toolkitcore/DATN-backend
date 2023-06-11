using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    public class view_example
    {
        public Guid? dictionary_id { get; set; }
        public Guid? example_id { get; set; }
        public string detail { get; set; }
        public string detail_html { get; set; }
        public string tone_name { get; set; }
        public string mode_name { get; set; }
        public string register_name { get; set; }
        public string nuance_name { get; set; }
        public string dialect_name { get; set; }
        public string note { get; set; }
        public DateTime? created_date { get; set; }
        public DateTime? modified_date { get; set; }
    }
}
