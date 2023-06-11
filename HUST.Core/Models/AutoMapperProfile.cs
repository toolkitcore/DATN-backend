using AutoMapper;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using HUST.Core.Models.ServerObject;

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
            CreateMap<sys_concept_link, SysConceptLink>().ReverseMap();
            //CreateMap<SysConceptLink, sys_concept_link>();

            CreateMap<sys_example_link, SysExampleLink>();
            CreateMap<sys_example_link, SysExampleLink>().ReverseMap();
            //CreateMap<SysExampleLink, sys_example_link>();

            CreateMap<sys_tone, SysTone>();
            CreateMap<sys_tone, SysTone>().ReverseMap();
            //CreateMap<SysTone, sys_tone>();

            CreateMap<sys_dialect, SysDialect>();
            CreateMap<sys_dialect, SysDialect>().ReverseMap();
            //CreateMap<SysDialect, sys_dialect>();

            CreateMap<sys_mode, SysMode>();
            CreateMap<sys_mode, SysMode>().ReverseMap();
            //CreateMap<SysMode, sys_mode>();

            CreateMap<sys_register, SysRegister>();
            CreateMap<sys_register, SysRegister>().ReverseMap();
            //CreateMap<SysRegister, sys_register>();

            CreateMap<sys_nuance, SysNuance>();
            CreateMap<sys_nuance, SysNuance>().ReverseMap();
            //CreateMap<SysNuance, sys_nuance>();
            #endregion

            #region Type
            CreateMap<concept_link, ConceptLink>();
            CreateMap<concept_link, ConceptLink>().ReverseMap();
            //CreateMap<ConceptLink, concept_link>();

            CreateMap<example_link, ExampleLink>();
            CreateMap<example_link, ExampleLink>().ReverseMap();
            //CreateMap<ExampleLink, example_link>();

            CreateMap<tone, Tone>();
            CreateMap<tone, Tone>().ReverseMap();
            //CreateMap<Tone, tone>();

            CreateMap<mode, Mode>();
            CreateMap<mode, Mode>().ReverseMap();
            //CreateMap<Mode, mode>();

            CreateMap<dialect, Dialect>();
            CreateMap<dialect, Dialect>().ReverseMap();
            //CreateMap<Dialect, dialect>();

            CreateMap<register, Register>();
            CreateMap<register, Register>().ReverseMap();
            //CreateMap<Register, register>();

            CreateMap<nuance, Nuance>();
            CreateMap<nuance, Nuance>().ReverseMap();
            //CreateMap<Nuance, nuance>();
            #endregion

            #region Relationship
            CreateMap<concept_relationship, ConceptRelationship>();
            CreateMap<concept_relationship, ConceptRelationship>().ReverseMap();
            //CreateMap<ConceptRelationship, concept_relationship>();

            CreateMap<example_relationship, ExampleRelationship>();
            CreateMap<example_relationship, ExampleRelationship>().ReverseMap();
            //CreateMap<ExampleRelationship, example_relationship>();
            #endregion

            #region General
            CreateMap<user, User>();
            CreateMap<user, User>().ReverseMap();
            //CreateMap<User, user>();

            CreateMap<dictionary, Dictionary>();
            CreateMap<dictionary, Dictionary>().ReverseMap();
            //CreateMap<Dictionary, dictionary>();

            CreateMap<concept, Concept>();
            CreateMap<concept, Concept>().ReverseMap();
            //CreateMap<Concept, concept>();

            CreateMap<example, Example>();
            CreateMap<example, Example>().ReverseMap();
            //CreateMap<Example, example>();

            CreateMap<user_setting, UserSetting>();
            CreateMap<user_setting, UserSetting>().ReverseMap();
            //CreateMap<UserSetting, user_setting>();

            CreateMap<audit_log, AuditLog>();
            CreateMap<audit_log, AuditLog>().ReverseMap();
            //CreateMap<AuditLog, audit_log>().ReverseMap();

            CreateMap<view_concept_relationship, ViewConceptRelationship>();
            CreateMap<view_concept_relationship, ViewConceptRelationship>().ReverseMap();

            CreateMap<view_example_relationship, ViewExampleRelationship>();
            CreateMap<view_example_relationship, ViewExampleRelationship>().ReverseMap();

            CreateMap<view_example, ViewExample>();
            CreateMap<view_example, ViewExample>().ReverseMap();

            // Import
            CreateMap<concept, ConceptImport>();
            CreateMap<view_example, ExampleImport>();
            CreateMap<view_concept_relationship, ConceptRelationshipImport>();
            CreateMap<view_example_relationship, ExampleRelationshipImport>();

            CreateMap<cache_sql, CacheSql>();
            CreateMap<cache_sql, CacheSql>().ReverseMap();

            CreateMap<cache_external_word_api, CacheExternalWordApi>();
            CreateMap<cache_external_word_api, CacheExternalWordApi>().ReverseMap();
            #endregion
        }


    }
}
