using HUST.Core.Models;
using HUST.Core.Models.Entity;
using HUST.Core.Models.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using HUST.Core.Constants;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace HUST.Core.Utils
{
    /// <summary>
    /// Chứa các hàm hỗ trợ
    /// </summary>
    public static class FunctionUtil
    {
        /// <summary>
        /// Hàm chuyển dữ liệu kiểu List sang kiểu DataTable
        /// Có thể dùng khi debug QuickWatch
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="dataTableName"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IList<T> items, string dataTableName = "")
        {
            var tableName = typeof(T).Name;
            if(!string.IsNullOrEmpty(dataTableName))
            {
                tableName = dataTableName;
            }

            DataTable dataTable = new DataTable(tableName);

            // Lấy tất cả properties
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var prop in props)
            {
                // Định nghĩa type của data column
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);

                // Đặt tên cột = tên property
                dataTable.Columns.Add(prop.Name, type);
            }

            foreach(T item in items)
            {
                var values = new object[props.Length];
                for(int i = 0; i < props.Length; ++i)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        /// <summary>
        /// Chuyển object sang byte[]
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this object obj)
        {
            if (obj == null)
            {
                return new byte[] { };
            }

            var str = SerializeUtil.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// Chuyên mảng byte sang object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T ToObject<T>(this byte[] bytes)
        {
            if (bytes == null)
            {
                return default(T);
            }

            var str = Encoding.UTF8.GetString(bytes);
            return SerializeUtil.DeserializeObject<T>(str);
        }

        /// <summary>
        /// Snakecase to Pascal case
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToPascalCase(this string str)
        {
            return str
                .Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
                .Aggregate(string.Empty, (s1, s2) => s1 + s2);

        }

        /// <summary>
        /// To snake case
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToSnakeCase(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        /// <summary>
        /// Lấy type lớp entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Type GetEntityType<T>()
        {
            //if (typeof(T).IsAssignableFrom(typeof(BaseDTO)))
            //{
            //    var dtoName = typeof(T).Name;
            //    var entityName = dtoName.ToSnakeCase();
            //    return Type.GetType(entityName);
            //}
            //return typeof(T);

            var dtoName = typeof(T).Name;
            var entityName = dtoName.ToSnakeCase();
            return Type.GetType($"HUST.Core.Models.Entity.{entityName},HUST.Core");
        }

        /// <summary>
        /// Lấy type lớp DTO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Type GetDTOType<T>()
        {
            if (typeof(T).IsAssignableFrom(typeof(BaseEntity)))
            {
                var entityName = typeof(T).Name;
                var dtoName = entityName.ToPascalCase();
                return Type.GetType(dtoName);
            }
            return typeof(T);
        }

        /// <summary>
        /// Lấy ra Type của model
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetModelType(string typeName)
        {
            var ns = string.Format(
                GlobalConfig.ModelNamespace.FirstOrDefault(x => Type.GetType(string.Format(x, typeName)) != null) ?? "{0}", typeName);
            return Type.GetType(ns);
        }

        /// <summary>
        /// Lấy trường key của DTO
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static PropertyInfo GetDtoKeyProperty(Type dtoType)
        {
            var prop = dtoType.GetProperties().FirstOrDefault(x => Attribute.IsDefined(x, typeof(Dapper.Contrib.Extensions.KeyAttribute)));
            return prop;
        }

        /// <summary>
        /// Lấy trường key của entity
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static PropertyInfo GetEntityKeyProperty(Type dtoType)
        {
            var prop = dtoType.GetProperties().FirstOrDefault(x => Attribute.IsDefined(x, typeof(System.ComponentModel.DataAnnotations.KeyAttribute)));
            return prop;
        }

        /// <summary>
        /// Kiểm tra kích thước file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="maxFileSize">Kích thước file tối đa - MB (defaut: 3 MiB, xấp xỉ 3.1 MB)</param>
        /// <returns></returns>
        public static bool IsValidFileSize(IFormFile file, double maxFileSize = 3)
        {
            if(file != null && file.Length < 1024*1024* maxFileSize)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Kiểm tra loại file có phải ảnh hay không
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsImageFile(IFormFile file)
        {
            if (file != null && FileContentType.Image.Contains(file.ContentType))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Chuẩn hóa từ
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string NormalizeText(string text)
        {
            if(string.IsNullOrEmpty(text))
            {
                return text;
            }
            text = text.Trim().ToLower();
            text = Regex.Replace(text, "[^0-9a-z ]", "");
            return text;
        }

        /// <summary>
        /// Lấy tên cột excel dựa trên chỉ số cột
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        public static string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";

            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }

        /// <summary>
        /// Loại bỏ thẻ html khỏi string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        /// <summary>
        /// Loại bỏ tất cả thẻ html trừ thẻ hightlight
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// PTHIEU 02.07.2023
        public static string StripHtmlExceptHightlight(string input)
        {
            if(string.IsNullOrEmpty(input))
            {
                return input;
            }
            return Regex.Replace(input, "</?(?!mark)\\w*\\b[^>]*>", string.Empty);
        }


        /// <summary>
        /// Kiểm tra chuỗi có đoạn highlight hay không
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckStringHasHightlight(string str)
        {
            //return true;
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            return Regex.IsMatch(str, "<mark.*>.*</mark>");
            
        }


        /// <summary>
        /// Sinh chuỗi highlight
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GenerateStringHightlight(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            return $"<mark>{str}</mark>";
        }
    }
}
