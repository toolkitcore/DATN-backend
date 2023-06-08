using HUST.Core.Constants;
using HUST.Core.Models.DTO;
using System.Collections.Generic;
using System.Linq;

namespace HUST.Core.Models.ServerObject
{
    /// <summary>
    /// Đối tượng xuất/nhập khẩu: map với concept
    /// </summary>
    public class ConceptImport : BaseImport
    {
        // Order không phải vị trí cột, mà là giá trị sắp xếp
        [ImportColumn(TemplateConfig.ConceptSheet.Title)]
        public string Title { get; set; }

        [ImportColumn(TemplateConfig.ConceptSheet.Description)]
        public string Description { get; set; }

        /// <summary>
        /// Validate nhập khẩu
        /// </summary>
        /// <param name="lstExistConcept"></param>
        /// <returns></returns>
        public ValidateResultImport ValidateBusinessMater(List<Concept> lstExistConcept)
        {
            var res = new ValidateResultImport
            {
                IsValid = true,
                Row = RowIndex,
                ListErrorMessage = new List<string>()
            };
            if (string.IsNullOrEmpty(Title))
            {
                res.IsValid = false;
                res.ListErrorMessage.Add(string.Format(ImportValidateErrorMessage.Required, "Title"));
            }
            else if (lstExistConcept.Any(x => x.Title == Title))
            {
                res.IsValid = false;
                res.ListErrorMessage.Add(string.Format(ImportValidateErrorMessage.Duplicated, "Title"));
            }

            return res;
        }
    }
}
