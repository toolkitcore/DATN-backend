using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
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

        /// <summary>
        /// Lấy danh sách concept trong từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> GetListConcept(string dictionaryId)
        {
            var res = new ServiceResult();

            if (string.IsNullOrEmpty(dictionaryId))
            {
                dictionaryId = this.ServiceCollection.AuthUtil.GetCurrentDictionaryId()?.ToString();
            }

            var lstConcept = await _repository.SelectObjects<Concept>(new Dictionary<string, object>
            {
                { nameof(concept.dictionary_id), dictionaryId }
            }) as List<Concept>;

            res.Data = new
            {
                ListConcept = lstConcept?.OrderBy(x => x.Title),
                LastUpdatedAt = DateTime.Now
            };

            return res;
        }

        /// <summary>
        /// Thêm 1 concept vào từ điển
        /// </summary>
        /// <param name="concept"></param>
        /// <returns></returns>
        public async Task<IServiceResult> AddConcept(Concept concept)
        {
            var res = new ServiceResult();

            if (concept == null || string.IsNullOrWhiteSpace(concept.Title))
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }

            if (concept.DictionaryId == null || concept.DictionaryId == Guid.Empty)
            {
                concept.DictionaryId = this.ServiceCollection.AuthUtil.GetCurrentDictionaryId();
            }

            concept.Title = concept.Title.Trim();
            concept.Description = concept.Description?.Trim();

            // Kiểm tra concept đã tồn tại trong từ điển
            var existConcept = await _repository.SelectObject<Concept>(new Dictionary<string, object>
            {
                { nameof(Models.Entity.concept.dictionary_id), concept.DictionaryId },
                { nameof(Models.Entity.concept.title), concept.Title }
            }) as Concept;

            if (existConcept != null)
            {
                return res.OnError(ErrorCode.Err3001, ErrorMessage.Err3001);
            }

            _ = await _repository.Insert(new concept
            {
                concept_id = Guid.NewGuid(),
                dictionary_id = concept.DictionaryId,
                title = concept.Title,
                description = concept.Description,
                created_date = DateTime.Now
            });

            return res;
        }

        /// <summary>
        /// Thực hiện cập nhật tên, mô tả của concept
        /// </summary>
        /// <param name="concept"></param>
        /// <returns></returns>
        public async Task<IServiceResult> UpdateConcept(Concept concept)
        {
            var res = new ServiceResult();

            if (concept == null || concept.ConceptId == Guid.Empty || string.IsNullOrWhiteSpace(concept.Title))
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }
            concept.Title = concept.Title.Trim();
            concept.Description = concept.Description?.Trim();

            // Lấy ra bản ghi đã lưu trong db
            var savedConcept = await _repository.SelectObject<Concept>(new Dictionary<string, object>
            {
                { nameof(Models.Entity.concept.concept_id), concept.ConceptId },
            }) as Concept;

            if (savedConcept == null)
            {
                return res.OnError(ErrorCode.Err3005, ErrorMessage.Err3005);
            }

            if (concept.Title != savedConcept.Title)
            {
                // Kiểm tra concept đã tồn tại trong từ điển
                var existConcept = await _repository.SelectObject<Concept>(new Dictionary<string, object>
                {
                    { nameof(Models.Entity.concept.dictionary_id), savedConcept.DictionaryId },
                    { nameof(Models.Entity.concept.title), concept.Title }
                }) as Concept;

                if (existConcept != null)
                {
                    return res.OnError(ErrorCode.Err3001, ErrorMessage.Err3001);
                }
            }

            // Cập nhật
            var result = await _repository.Update(new
            {
                concept_id = concept.ConceptId,
                title = concept.Title,
                description = concept.Description,
                modified_date = DateTime.Now
            });

            if(!result)
            {
                return res.OnError(ErrorCode.Err9999);
            }

            return res;
        }

        /// <summary>
        /// Thực hiện xóa concept
        /// </summary>
        /// <param name="conceptId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> DeleteConcept(string conceptId)
        {
            var res = new ServiceResult();

            if(string.IsNullOrEmpty(conceptId))
            {
                return res.OnError(ErrorCode.Err9999);
            }

            // Kiểm tra concept có liên kết với example hay không
            var linkedExample = await _repository.SelectObjects<ExampleRelationship>(new
            {
                concept_id = conceptId
            }) as List<ExampleRelationship>;

            if(linkedExample != null && linkedExample.Count > 0)
            {
                return res.OnError(ErrorCode.Err3002, ErrorMessage.Err3002, data: linkedExample.Count);
            }

            // Thực hiện xóa concept
            var result = await _repository.Delete(new
            {
                concept_id = conceptId
            });

            if(!result)
            {
                return res.OnError(ErrorCode.Err3005, ErrorMessage.Err3005);
            }

            return res;
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
            int[,] linkStrengthArr = new int[n, n];
            for (var i = 0; i < n; ++i)
            {
                for (var j = 0; j <= i; ++j)
                {
                    if (i == j)
                    {
                        linkStrengthArr[i, j] = LinkStrength.Itself;
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
            for (var i = 0; i < n; ++i)
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

            bool Stop(List<decimal> arr1, List<decimal> arr2)
            {
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
