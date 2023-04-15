//using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace HUST.Core.Utils
{
    /// <summary>
    /// Lớp xử lý việc ghi log (sử dụng NLog)
    /// Nhược điểm là thông tin class Logger sẽ luôn  hiển thị là HUST.Core.Utils.LogUtil 
    /// </summary>
    public class LogUtil : ILogUtil
    {
        #region Declaration

        private readonly ILogger _logger;

        #endregion

        #region Constructor
        
        public LogUtil()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }
        #endregion

        #region Methods
        public void LogTrace(string msg)
        {
            _logger.Log(LogLevel.Trace, msg);
        }

        public void LogDebug(string msg)
        {
            _logger.Log(LogLevel.Debug, msg);
        }

        public void LogInfo(string msg)
        {
            _logger.Log(LogLevel.Info, msg);
        }

        public void LogWarn(string msg)
        {
            _logger.Log(LogLevel.Warn, msg);
        }

        public void LogError(string msg)
        {
            _logger.Log(LogLevel.Error, msg);
        }

        public void LogError(Exception ex, string msg)
        {
            _logger.Log(LogLevel.Error, ex, msg);
        }

        #endregion

    }
}
