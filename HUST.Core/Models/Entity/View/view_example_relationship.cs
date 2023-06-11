using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    public class view_example_relationship
    {
        public Guid? dictionary_id { get; set; }
        public Guid? concept_id { get; set; }
        public string concept { get; set; }
        public Guid? example_id { get; set; }
        public string example { get; set; }
        public string example_html { get; set; }
        public Guid? example_link_id { get; set; }
        public string example_link_name { get; set; }
        public DateTime? concept_created_date { get; set; }
        public DateTime? example_created_date { get; set; }
        public DateTime? relation_created_date { get; set; }
    }
}
