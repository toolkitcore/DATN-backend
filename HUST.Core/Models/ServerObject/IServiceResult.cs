using HUST.Core.Enums;
using System;

namespace HUST.Core.Models.ServerObject
{
    /// <summary>
    /// Kết quả thực hiện service
    /// </summary>
    public interface IServiceResult
    {
        #region Properties
        /// <summary>
        /// Kết quả thực hiện
        /// </summary>
        public ServiceResultStatus Status { get; set; }

        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Thông báo
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Thông báo cho dev
        /// </summary>
        public string DevMessage { get; set; }

        /// <summary>
        /// Mã lỗi (string)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Mã lỗi (số)
        /// </summary>
        public int ErrorCode { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Hàm gọi khi thực hiện thành công
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        IServiceResult OnSuccess(object data = null, string message = null);

        /// <summary>
        /// Hàm gọi khi thực hiện có lỗi
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        IServiceResult OnError(int errorCode, string message = null, string code = null, object data = null);

        /// <summary>
        /// Hàm gọi khi thực hiện bị exception
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        IServiceResult OnException(Exception exception, string message = null);
        #endregion
    }
}
