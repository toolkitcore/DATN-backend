using Microsoft.Extensions.DependencyInjection;
using HUST.Core.Models;

namespace HUST.Core.Extensions
{
    /// <summary>
    /// Extension cho auto mapper
    /// </summary>
    public static class MapperExtension
    {
        public static void UseAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
        }
    }
}
