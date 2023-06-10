using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Models.Param
{
    /// <summary>
    /// Param tìm kiếm example
    /// </summary>
    public class SearchExampleParam
    {
        public Guid? DictionaryId { get; set; }
        public string Keyword { get; set; }
        public string ToneId { get; set; }
        public string ModeId { get; set; }
        public string RegisterId { get; set; }
        public string NuanceId { get; set; }
        public string DialectId { get; set; }

        public List<string> ListLinkedConceptId { get; set; }

        /// <summary>
        /// Tìm kiếm example không liên kết concept
        /// </summary>
        public bool? IsSearchUndecided { get; set; }

        /// <summary>
        /// Sử dụng fulltext search trong mysql
        /// </summary>
        public bool? IsFulltextSearch { get; set; }
    }
}
