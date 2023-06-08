using HUST.Core.Enums;
using HUST.Core.Models.Entity;
using System;
using System.Collections.Generic;

namespace HUST.Core.Models.ServerObject
{
    /// <summary>
    /// Dữ liệu config: concept link, example link, tone, mode, register, nuance, dialect
    /// </summary>
    public class UserConfigData
    {
        public List<concept_link> ListConceptLink { get; set; }
        public List<example_link> ListExampleLink { get; set; }
        public List<tone> ListTone { get; set; }
        public List<mode> ListMode { get; set; }
        public List<register> ListRegister { get; set; }
        public List<nuance> ListNuance { get; set; }
        public List<dialect> ListDialect { get; set; }
    }
}
