using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Models.Param
{
    /// <summary>
    /// Param cho api update_concept_relationship
    /// </summary>
    public class UpdateConceptRelationshipParam
    {
        public Guid ConceptId { get; set; }
        public Guid ParentId { get; set; }
        public Guid? ConceptLinkId { get; set; }
        public bool? IsForced { get; set; }

    }
}
