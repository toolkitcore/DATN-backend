using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Constants
{
    /// <summary>
    /// Cấu hình global namespace
    /// </summary>
    public static class GlobalConfig
    {
        public static string Environment = null;
        public static bool IsDevelopment = false;
        public static string ContentRootPath = "";

        public static readonly int ConnectionTimeout = 999;

        public static readonly string[] ModelNamespace =
        { 
            "HUST.Core.Models.DTO.{0}, HUST.Core",
            "HUST.Core.Models.Entity.{0}, HUST.Core",
        };

        public static readonly string[] ServiceNamespace =
        {
            "HUST.Core.Models.Services.{0}, HUST.Core",
        };

        public static readonly string[] RepositoryNamespace =
        {
            "HUST.Infrastructure.Repositories.{0}, HUST.Infrastructure",
        };
    }

}
