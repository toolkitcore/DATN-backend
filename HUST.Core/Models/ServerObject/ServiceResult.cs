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

        public IServiceResult OnSuccess(object data)
        {
            this.Status = ServiceResultStatus.Success;
            this.Data = data;
            return this;
        }

        public IServiceResult OnError(int errorCode, object data = null, string code = "")
        {
            this.Status = ServiceResultStatus.Fail;
            this.Data = data;
            this.ErrorCode = errorCode;
            this.Code = code;
            return this;
        }

        public IServiceResult OnException(Exception exception, string message = "")
        {
            this.Status = ServiceResultStatus.Exception;
            this.Message = $"{this.Message} {exception.Message}";
            return this;
        }
        #endregion
    }
}
