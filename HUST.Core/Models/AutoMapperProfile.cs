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

            #region System
            CreateMap<sys_concept_link, SysConceptLink>();
            CreateMap<SysConceptLink, sys_concept_link>();

            CreateMap<sys_example_link, SysExampleLink>();
            CreateMap<SysExampleLink, sys_example_link>();

            CreateMap<sys_tone, SysTone>();
            CreateMap<SysTone, sys_tone>();

            CreateMap<sys_dialect, SysDialect>();
            CreateMap<SysDialect, sys_dialect>();

            CreateMap<sys_mode, SysMode>();
            CreateMap<SysMode, sys_mode>();

            CreateMap<sys_register, SysRegister>();
            CreateMap<SysRegister, sys_register>();

            CreateMap<sys_nuance, SysNuance>();
            CreateMap<SysNuance, sys_nuance>();
            #endregion

            #region Type
            CreateMap<concept_link, ConceptLink>();
            CreateMap<ConceptLink, concept_link>();

            CreateMap<example_link, ExampleLink>();
            CreateMap<ExampleLink, example_link>();

            CreateMap<tone, Tone>();
            CreateMap<Tone, tone>();

            CreateMap<mode, Mode>();
            CreateMap<Mode, mode>();

            CreateMap<dialect, Dialect>();
            CreateMap<Dialect, dialect>();

            CreateMap<register, Register>();
            CreateMap<Register, register>();

            CreateMap<nuance, Nuance>();
            CreateMap<Nuance, nuance>();
            #endregion

            #region Relationship
            CreateMap<concept_relationship, ConceptRelationship>();
            CreateMap<ConceptRelationship, concept_relationship>();

            CreateMap<example_relationship, ExampleRelationship>();
            CreateMap<ExampleRelationship, example_relationship>();
            #endregion

            #region General
            CreateMap<user, User>();
            CreateMap<User, user>();

            CreateMap<dictionary, Dictionary>();
            CreateMap<Dictionary, dictionary>();

            CreateMap<concept, Concept>();
            CreateMap<Concept, concept>();

            CreateMap<example, Example>();
            CreateMap<Example, example>();

            CreateMap<user_setting, UserSetting>();
            CreateMap<UserSetting, user_setting>();

            CreateMap<audit_log, AuditLog>();
            CreateMap<AuditLog, audit_log>();
            #endregion
        }
    }
}
