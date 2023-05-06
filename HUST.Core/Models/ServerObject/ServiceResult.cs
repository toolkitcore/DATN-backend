using HUST.Core.Enums;
using System;

namespace HUST.Core.Models.ServerObject
{
    /// <summary>
    /// Kết quả thực hiện service
    /// </summary>
    public class ServiceResult : IServiceResult
    {
        #region Properties
        /// <summary>
        /// Kết quả thực hiện
        /// </summary>
        public ServiceResultStatus Status { get; set; } = ServiceResultStatus.Success;

        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Thông báo
        /// </summary>
        public string Message { get; set; }

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

        public IServiceResult OnSuccess(object data = null, string message = null)
        {
            this.Status = ServiceResultStatus.Success;
            this.Data = data;
            this.Message = message;
            return this;
        }

        public IServiceResult OnError(int errorCode, string message = null, string code = null, object data = null)
        {
            this.Status = ServiceResultStatus.Fail;
            this.Data = data;
            this.ErrorCode = errorCode;
            this.Message = message;
            this.Code = code;
            return this;
        }

        public IServiceResult OnException(Exception exception, string message = null)
        {
            this.Status = ServiceResultStatus.Exception;
            this.Message = $"{this.Message} {exception.Message}";
            return this;
        }
        #endregion
    }
}
