using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Utils
{
    public interface ILogUtil
    {
        void LogTrace(string msg);

        void LogDebug(string msg);

        void LogInfo(string msg);

        void LogWarn(string msg);

        void LogError(string msg);

        void LogError(Exception ex, string msg);




    }
}
