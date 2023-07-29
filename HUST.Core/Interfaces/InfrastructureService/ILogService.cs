using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.InfrastructureService
{
    public interface ILogService
    {
        void LogTrace(string msg);

        void LogDebug(string msg);

        void LogInfo(string msg);

        void LogWarn(string msg);

        void LogError(string msg);

        void LogError(Exception ex, string msg);




    }
}
