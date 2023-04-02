using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

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
    }
}
