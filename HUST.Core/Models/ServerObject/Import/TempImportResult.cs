using HUST.Core.Enums;
using System;
using System.Collections.Generic;

namespace HUST.Core.Models.ServerObject
{
    /// <summary>
    /// Kết quả bước đầu (sau validate) nhập khẩu
    /// </summary>
    public class TempImportResult
    {
        public string ImportSession { get; set; }
        public int NumberValid { get; set; }
        public int NumberError { get; set; }
        public List<ValidateResultImport> ListValidateError { get; set; }
        public string StrFileError { get; set; }
    }
}
