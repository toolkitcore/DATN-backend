using HUST.Core.Constants;

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
    }
}
