using HUST.Core.Constants;

namespace HUST.Core.Models.ServerObject
{
    /// <summary>
    /// Đối tượng xuất/nhập khẩu: map với view_example
    /// </summary>
    public class ExampleImport : BaseImport
    {
        [ImportColumn(TemplateConfig.ExampleSheet.Example)]
        public string DetailHtml { get; set; }

        [ImportColumn(TemplateConfig.ExampleSheet.Tone)]
        public string ToneName { get; set; }

        [ImportColumn(TemplateConfig.ExampleSheet.Mode)]
        public string ModeName { get; set; }

        [ImportColumn(TemplateConfig.ExampleSheet.Register)]
        public string RegisterName { get; set; }

        [ImportColumn(TemplateConfig.ExampleSheet.Nuance)]
        public string NuanceName { get; set; }

        [ImportColumn(TemplateConfig.ExampleSheet.Dialect)]
        public string DialectName { get; set; }

        [ImportColumn(TemplateConfig.ExampleSheet.Note)]
        public string Note { get; set; }
    }
}
