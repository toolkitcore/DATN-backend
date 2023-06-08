using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Models.ServerObject
{
    /// <summary>
    /// Kết quả validate nhập khẩu
    /// </summary>
    public class ValidateResultImport
    {
        public bool? IsValid { get; set; }
        public int SheetIndex { get; set; }
        public string SheetName { get; set; }
        public int Row { get; set; }
        public List<string> ListErrorMessage { get; set; }

        public string ErrorMessage
        {
            get
            {
                return string.Join("; ", this.ListErrorMessage);
            }
        }
    }
}
