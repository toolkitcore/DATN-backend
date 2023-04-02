using AutoMapper;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;

namespace HUST.Core.Models
{
    /// <summary>
    /// Khai báo các cấu hình map giữa entity, DTO
    /// </summary>
    /// Created by pthieu 30.03.2023
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
            DestinationMemberNamingConvention = new PascalCaseNamingConvention();

            CreateMap<user, User>();
            CreateMap<User, user>();

        }
    }
}
