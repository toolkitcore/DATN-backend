using Newtonsoft.Json;
using System;

namespace HUST.Core.Utils
{
    /// <summary>
    /// Lớp xử lý các hành động chuyển dữ liệu giữa Object và String
    /// </summary>
    public static class SerializeUtil
    {
        public static readonly DateFormatHandling JSONDateFormatHandling = DateFormatHandling.IsoDateFormat;
        public static readonly DateTimeZoneHandling JSONDateTimeZoneHandling = DateTimeZoneHandling.Local;
        public static readonly string JSONDateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffFFFFK";
        public static readonly NullValueHandling JSONNullValueHandling = NullValueHandling.Include;
        public static readonly ReferenceLoopHandling JSONReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        /// <summary>
        /// Custom lại format
        /// </summary>
        /// <returns></returns>
        public static JsonSerializerSettings GetJsonSerializerSetting()
        {
            return new JsonSerializerSettings()
            {
                DateFormatHandling = JSONDateFormatHandling,
                DateTimeZoneHandling = JSONDateTimeZoneHandling,
                DateFormatString = JSONDateFormatString,
                NullValueHandling = JSONNullValueHandling,
                ReferenceLoopHandling = JSONReferenceLoopHandling
            };
        }

        /// <summary>
        /// Hàm chuyển object sang json
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SerializeObject(object data)
        {
            return JsonConvert.SerializeObject(data, GetJsonSerializerSetting());
        }

        /// <summary>
        /// Hàm chuyển json sang object thuộc kiểu T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, GetJsonSerializerSetting());
        }

        /// <summary>
        /// Hàm chuyển json sang object
        /// </summary>
        /// <param name="data"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static object DeserializeObject(string data, Type objectType)
        {
            return JsonConvert.DeserializeObject(data, objectType, GetJsonSerializerSetting());
        }

    }
}