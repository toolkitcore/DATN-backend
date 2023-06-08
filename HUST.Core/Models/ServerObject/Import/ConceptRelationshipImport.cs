using HUST.Core.Constants;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using System.Collections.Generic;
using System.Linq;

namespace HUST.Core.Models.ServerObject
{
    /// <summary>
    /// Đối tượng xuất/nhập khẩu: map với view_concept_relationship
    /// </summary>
    public class ConceptRelationshipImport : BaseImport
    {
        [ImportColumn(TemplateConfig.ConceptRelationshipSheet.ChildConcept)]
        public string ChildName { get; set; }

        [ImportColumn(TemplateConfig.ConceptRelationshipSheet.ParentConcept)]
        public string ParentName { get; set; }

        [ImportColumn(TemplateConfig.ConceptRelationshipSheet.Relation)]
        public string ConceptLinkName { get; set; }

        /// <summary>
        /// Validate nhập khẩu
        /// </summary>
        /// <param name="lstExistConceptRel">Danh sách liên kết đã tồn tại</param>
        /// <param name="findChildConcept">Đối tượng concept con</param>
        /// <param name="findParentConcept">Đối tượng concept cha</param>
        /// <param name="findRelation">Đối tượng concept link</param>
        /// <returns></returns>
        public ValidateResultImport ValidateBusinessMater(List<ConceptRelationship> lstExistConceptRel, 
            Concept findChildConcept, 
            Concept findParentConcept,
            concept_link findRelation)
        {
            var res = new ValidateResultImport
            {
                IsValid = true,
                Row = RowIndex,
                ListErrorMessage = new List<string>()
            };

            // Validate concept con
            if (string.IsNullOrEmpty(ChildName))
            {
                res.IsValid = false;
                res.ListErrorMessage.Add(string.Format(ImportValidateErrorMessage.Required, "Child concept"));
            }
            else if (findChildConcept == null)
            {
                res.IsValid = false;
                res.ListErrorMessage.Add(string.Format(ImportValidateErrorMessage.NotExist, "Child concept"));
            }

            // Validate concept cha
            if (string.IsNullOrEmpty(ParentName))
            {
                res.IsValid = false;
                res.ListErrorMessage.Add(string.Format(ImportValidateErrorMessage.Required, "Parent concept"));
            }
            else if (findParentConcept == null)
            {
                res.IsValid = false;
                res.ListErrorMessage.Add(string.Format(ImportValidateErrorMessage.NotExist, "Parent concept"));
            }

            // Validate lỗi liên kết chính mình
            if (!string.IsNullOrEmpty(ChildName) && ChildName == ParentName)
            {
                res.IsValid = false;
                res.ListErrorMessage.Add(ImportValidateErrorMessage.ConceptLinkToItself);
            }

            // Validate loại liên kết
            if (string.IsNullOrEmpty(ConceptLinkName))
            {
                res.IsValid = false;
                res.ListErrorMessage.Add(string.Format(ImportValidateErrorMessage.Required, "Relation"));
            }
            else if (findRelation == null)
            {
                res.IsValid = false;
                res.ListErrorMessage.Add(string.Format(ImportValidateErrorMessage.NotExist, "Relation"));
            }

            if(findChildConcept != null && findParentConcept != null)
            {
                // Validate duplicate
                if (lstExistConceptRel.Any(x => x.ConceptId == findChildConcept.ConceptId && x.ParentId == findParentConcept.ConceptId))
                {
                    res.IsValid = false;
                    res.ListErrorMessage.Add(string.Format(ImportValidateErrorMessage.Duplicated, "Row"));
                }

                // Validate circle
                if (lstExistConceptRel.Any(x => x.ConceptId == findParentConcept.ConceptId && x.ParentId == findChildConcept.ConceptId))
                {
                    res.IsValid = false;
                    res.ListErrorMessage.Add(ImportValidateErrorMessage.ConceptCircleLink);
                }
            }

            return res;
        }
    }
}
