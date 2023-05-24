using HUST.Core.Models.DTO;
using HUST.Core.Models.ServerObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Service
{

    public interface IConceptService: IBaseService<Concept>
    {
        Task<ServiceResult> GetListRecommendConcept(List<string> keywords, Guid dictionaryId);
    }
}
