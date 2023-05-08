using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Services
{
    /// <summary>
    /// Serivce xử lý account
    /// </summary>
    public class ConceptService : BaseService, IConceptService
    {
        #region Field

        private readonly IConceptRepository _repository;

        #endregion

        #region Constructor

        public ConceptService(IConceptRepository repository,
            IHustServiceCollection serviceCollection) : base(serviceCollection)
        {
            _repository = repository;
        }

        

        #endregion

        #region Method
       

        public Task<ServiceResult> Delete(Guid entityId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> DeleteMany(List<Guid> entityIds)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> GetById(Guid entityId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> Insert(Concept entity)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> Update(Guid entityId, Concept entity)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult> GetListRecommendConcept(List<string> keywords, Guid dictionaryId)
        {
            var res = new ServiceResult();


            //var dictionaryId = this.ServiceCollection.AuthUtil.GetCurrentDictionaryId();

            var conceptData = await _repository.SelectObjects<Concept>(new { dictionary_id = dictionaryId }) as List<Concept>;
            var conceptRelationshipData = await _repository.SelectObjects<ConceptRelationship>(new { dictionary_id = dictionaryId }) as List<ConceptRelationship>;


            var data = (from r in conceptRelationshipData
                       join c1 in conceptData on r.ConceptId equals c1.ConceptId
                       join c2 in conceptData on r.ParentId equals c2.ConceptId
                       select new
                       {
                           Concept1 = c1.Title,
                           Concept2 = c2.Title
                       }).ToList();

            var lstConcept = new List<string>();
            var deepLevel = 1;

            void Find(List<string> words, int k)
            {
                if (k > deepLevel)
                {
                    return;
                }

                var lstLinkedConcept = new List<string>();
                foreach (var w in words)
                {
                    if (lstConcept.Contains(w))
                    {
                        continue;
                    }
                    lstConcept.Add(w);
                    //var concept = conceptData.FirstOrDefault(x => x.Title == w);
                    //if (concept != null)
                    //{
                    //    var lstLinkedConceptId = conceptRelationshipData
                    //        .Where(x => x.ConceptId == concept.ConceptId || x.ParentId == concept.ConceptId)
                    //        .Select(x => x.ConceptId == concept.ConceptId ? x.ParentId : x.ConceptId);
                    //    lstLinkedConcept.AddRange(conceptData.Where(x => lstLinkedConceptId.Contains(x.ConceptId)).Select(x => x.Title));
                    //}
                    lstLinkedConcept.AddRange(data.Where(x => x.Concept1 == w).Select(x => x.Concept2));
                    lstLinkedConcept.AddRange(data.Where(x => x.Concept2 == w).Select(x => x.Concept1));
                }

                Find(lstLinkedConcept, k + 1);
            }

            Find(keywords, 0);

            //var dictRelationship = new Dictionary<Concept, List<Concept>>();
            //foreach(var keyword in keywords)
            //{
            //    var concept = conceptData.FirstOrDefault(x => x.Title == keyword);
            //    if (concept != null)
            //    {
            //        var lstLinkedConceptId = conceptRelationshipData
            //            .Where(x => x.ConceptId == concept.ConceptId || x.ParentId == concept.ConceptId)
            //            .Select(x => x.ConceptId == concept.ConceptId ? x.ParentId : x.ConceptId);
            //        if (lstLinkedConceptId != null && lstLinkedConceptId.Any())
            //        {
            //            var lstLinkedConcept = conceptData.Where(x => lstLinkedConceptId.Contains(x.ConceptId));
            //        }
            //}
            bool isPrimary(string w)
            {
                return keywords.Contains(w);
            }

            bool isAssociated(string w1, string w2)
            {
                return data.Any(x => (x.Concept1 == w1 && x.Concept2 == w2) || (x.Concept1 == w2 && x.Concept2 == w1));
            }

            var n = lstConcept.Count;
            int[,] linkStrengthArr = new int[n,n];
            for(var i = 0; i < n; ++i)
            {
                for(var j = 0; j <= i; ++j)
                {
                    if(i == j)
                    {
                        linkStrengthArr[i,j] = LinkStrength.Itself;
                        continue;
                    } 

                    if (isPrimary(lstConcept[i]) && isPrimary(lstConcept[j]))
                    {
                        linkStrengthArr[i, j] = LinkStrength.TwoPrimary;
                        linkStrengthArr[j, i] = LinkStrength.TwoPrimary;
                        continue;
                    }

                    var hasLink = isAssociated(lstConcept[i], lstConcept[j]);

                    if ((isPrimary(lstConcept[i]) || isPrimary(lstConcept[j])) && hasLink)
                    {
                        linkStrengthArr[i, j] = LinkStrength.PrimaryAndAssociatedNonPrimary;
                        linkStrengthArr[j, i] = LinkStrength.PrimaryAndAssociatedNonPrimary;
                        continue;
                    }

                    if (hasLink)
                    {
                        linkStrengthArr[i, j] = LinkStrength.TwoAssociatedNonPrimary;
                        linkStrengthArr[j, i] = LinkStrength.TwoAssociatedNonPrimary;
                        continue;
                    }

                    if (isPrimary(lstConcept[i]) || isPrimary(lstConcept[j]))
                    {
                        linkStrengthArr[i, j] = LinkStrength.PrimaryAndUnAssociatedNonPrimary;
                        linkStrengthArr[j, i] = LinkStrength.PrimaryAndUnAssociatedNonPrimary;
                        continue;
                    }

                    linkStrengthArr[i, j] = LinkStrength.TwoUnassociatedNonPrimary;
                    linkStrengthArr[j, i] = LinkStrength.TwoUnassociatedNonPrimary;
                    continue;
                }
            }

            var lstActivateScore = Run(n, linkStrengthArr);
            var myDict = new Dictionary<string, decimal>();
            for(var i = 0; i < n; ++i)
            {
                myDict.Add(lstConcept[i], lstActivateScore[i]);
            }

            res.Data = from entry in myDict orderby entry.Value descending select entry;
            return res;
        }
        #endregion


        #region Helper

        public List<decimal> Run(int size, int[,] linkStrengthArr)
        {
            decimal threshold = 0.01M;
            int n = 300;

            bool Stop (List<decimal> arr1, List<decimal> arr2) {
                for (var i = 0; i < arr1.Count; ++i)
                {
                    if (Math.Abs(arr1[i] - arr2[i]) > threshold)
                    {
                        return false;
                    }
                }

                return true;
            }

            var res = Enumerable.Repeat(1.0M, size).ToList();
            while (n > 0)
            {
                var tmp = res.Select((x, i) =>
                {
                    var sum = 0.0M;
                    for (var j = 0; j < res.Count; j++)
                    {
                        sum += res[j] * linkStrengthArr[i, j];
                    }
                    return sum;
                });

                var max = tmp.Max();

                var resTmp = tmp.Select(x => x / max).ToList();
                if (Stop(res, resTmp))
                {
                    res = resTmp;
                    break;
                }

                res = resTmp;
                n--;
            }

            return res;
        }
        #endregion

    }
}
