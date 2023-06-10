using HUST.Core.Constants;
using OfficeOpenXml.Attributes;
using System;

namespace HUST.Core.Models
{
    /// <summary>
    /// Thuộc tính xác định tên hiển thị của Property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MyDisplayNameAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public MyDisplayNameAttribute(string name)
        {
            this.DisplayName = name;
        }
    }

    /// <summary>
    /// Thuộc tính xác định Property là trường bắt buộc (không được để trống)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MyRequiredAttribute : Attribute
    {
    }

    /// <summary>
    /// Thuộc tính xác định Property là trường duy nhất (không được phép trùng)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MyUniqueAttribute : Attribute
    {
    }

    /// <summary>
    /// Thuộc tính xác định Property là trường email
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MyEmailAttribute : Attribute
    {
    }

    /// <summary>
    /// Thuộc tính xác định Property là trường số (chỉ chấp nhận chữ số)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MyNumberAttribute : Attribute
    {
    }

    /// <summary>
    /// Thuộc tính xác định độ dài tối đa
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MyMaxLengthAttribute : Attribute
    {
        public int MaxLength { get; set; }

        public MyMaxLengthAttribute(int maxLength)
        {
            MaxLength = maxLength;
        }
    }

    /// <summary>
    /// Thuộc tính xác định vị trí cột map mẫu nhập khẩu
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ImportColumn : EpplusTableColumnAttribute
    {
        public int ColumnIndex { get; set; }


        public ImportColumn(int column) : base()
        {
            ColumnIndex = column;
            Order = column - TemplateConfig.StartColData;
        }
    }
}
