using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Models.Param
{
    /// <summary>
    /// Param cho api add_dictionary
    /// </summary>
    public class AddDictionaryParam
    {
        public string DictionaryName { get; set; }
        public string CloneDictionaryId { get; set; }
    }
}
