using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Models.Param
{
    /// <summary>
    /// Param cho api transfer_dictionary
    /// </summary>
    public class TransderDictionaryParam
    {
        public string SourceDictionaryId { get; set; }
        public string DestDictionaryId { get; set; }

        /// <summary>
        /// Có xóa dữ liệu trước khi chuyển dữ liệu hay không
        /// </summary>
        public bool? IsDeleteData { get; set; }
    }
}
