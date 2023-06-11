using Dapper.Contrib.Extensions;
using System;

namespace HUST.Core.Models.Entity
{
    public class view_concept_relationship
    {
        public Guid? dictionary_id { get; set; }
        public Guid? child_id { get; set; }
        public string child_name { get; set; }
        public Guid? parent_id { get; set; }
        public string parent_name { get; set; }
        public Guid? concept_link_id { get; set; }
        public string concept_link_name { get; set; }
        public DateTime? child_created_date { get; set; }
        public DateTime? parent_created_date { get; set; }
        public DateTime? relation_created_date { get; set; }
    }
}
