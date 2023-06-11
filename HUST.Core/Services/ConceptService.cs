using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using HUST.Core.Models.Param;
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
        private readonly IConceptRelationshipRepository _conceptRelRepository;


        #endregion

        #region Constructor

        public ConceptService(IConceptRepository repository,
            IConceptRelationshipRepository conceptRelRepository,
            IHustServiceCollection serviceCollection) : base(serviceCollection)
        {
            _repository = repository;
            _conceptRelRepository = conceptRelRepository;
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

            if (!result)
            {
                return res.OnError(ErrorCode.Err9999);
            }

            return res;
        }

        /// <summary>
        /// Thực hiện xóa concept
        /// </summary>
        /// <param name="conceptId"></param>
        /// <param name="isForced"></param>
        /// <returns></returns>
        public async Task<IServiceResult> DeleteConcept(string conceptId, bool? isForced)
        {
            var res = new ServiceResult();

            if (string.IsNullOrEmpty(conceptId))
            {
                return res.OnError(ErrorCode.Err9999);
            }

            // Kiểm tra concept có liên kết với example hay không
            if (isForced != true)
            {
                var linkedExample = await _repository.SelectObjects<ExampleRelationship>(new
                {
                    concept_id = conceptId
                }) as List<ExampleRelationship>;

                if (linkedExample != null && linkedExample.Count > 0)
                {
                    return res.OnError(ErrorCode.Err3002, ErrorMessage.Err3002, data: new { NumberExample = linkedExample.Count }
                    );
                }
            }

            // Thực hiện xóa concept
            var result = await _repository.Delete(new
            {
                concept_id = conceptId
            });

            if (!result)
            {
                return res.OnError(ErrorCode.Err3005, ErrorMessage.Err3005);
            }

            return res;
        }

        /// <summary>
        /// Lấy dữ liệu concept và các example liên kết với concept đó
        /// </summary>
        /// <param name="conceptId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> GetConcept(string conceptId)
        {
            var res = new ServiceResult();

            if (string.IsNullOrEmpty(conceptId))
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }

            var tables = new string[]
            {
                nameof(Models.Entity.concept),
                nameof(view_example_relationship)
            };

            // Không dùng từ "params"
            var param = new Dictionary<string, Dictionary<string, object>>()
            {
                {
                    nameof(Models.Entity.concept),
                    new Dictionary<string, object> { { nameof(Models.Entity.concept.concept_id), conceptId } }
                },
                {
                    nameof(view_example_relationship),
                    new Dictionary<string, object> { { nameof(view_example_relationship.concept_id), conceptId } }
                }
            };

            var queryRes = await _repository.SelectManyObjects(tables, param) as Dictionary<string, object>;

            if (queryRes == null)
            {
                return res.OnSuccess(null);
            }

            var concept = (queryRes[nameof(Models.Entity.concept)] as List<Models.Entity.concept>)?.FirstOrDefault();
            var lstViewExampleRel = queryRes[nameof(view_example_relationship)] as List<view_example_relationship>;

            // TODO: sort theo rule như thế nào?
            var lstExample = lstViewExampleRel?.Select(x => new
            {
                ExampleId = x.example_id,
                Detail = x.example,
                DetailHtml = x.example_html,
                ExampleLinkId = x.example_link_id,
                ExampleLinkName = x.example_link_name
            }).OrderBy(x => x.Detail).ToList();

            return res.OnSuccess(new
            {
                ConceptId = concept?.concept_id,
                Title = concept?.title,
                Description = concept?.description,
                NormalizedTitle = concept?.normalized_title,
                ListExample = lstExample
            });
        }

        /// <summary>
        /// Lấy danh sách concept trong từ điển mà khớp với xâu tìm kiếm của người dùng
        /// </summary>
        /// <param name="searchKey"></param>
        /// <param name="dictionaryId"></param>
        /// <param name="isSearchSoundex"></param>
        /// <returns></returns>
        public async Task<IServiceResult> SearchConcept(string searchKey, string dictionaryId, bool? isSearchSoundex)
        {
            var res = new ServiceResult();
            if (string.IsNullOrEmpty(dictionaryId))
            {
                dictionaryId = this.ServiceCollection.AuthUtil.GetCurrentDictionaryId()?.ToString();
            }
            searchKey = FunctionUtil.NormalizeText(searchKey);
            res.Data = (await _repository.SearchConcept(searchKey, dictionaryId, isSearchSoundex)).OrderBy(x => x.Title);

            return res;
        }

        /// <summary>
        /// Lấy ra mối quan hệ liên kết giữa concept con và concept cha.
        /// </summary>
        /// <param name="conceptId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> GetConceptRelationship(string conceptId, string parentId)
        {
            var res = new ServiceResult();

            if (string.IsNullOrEmpty(conceptId) || string.IsNullOrEmpty(parentId))
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }

            var relation = await _repository.SelectObject<ViewConceptRelationship>(new Dictionary<string, object>
            {
                { nameof(view_concept_relationship.child_id), conceptId },
                { nameof(view_concept_relationship.parent_id), parentId },
            }) as ViewConceptRelationship;

            if (relation != null)
            {
                res.Data = new
                {
                    relation.ConceptLinkId,
                    relation.ConceptLinkName
                };
            }
            else
            {
                res.Data = new
                {
                    ConceptLinkId = Guid.Empty,
                    ConceptLinkName = "No link"
                };
            }
            return res;
        }

        /// <summary>
        /// Thực hiện cập nhật (hoặc tạo mới nếu chưa có) liên kết giữa 2 concept
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<IServiceResult> UpdateConceptRelationship(UpdateConceptRelationshipParam param)
        {
            var res = new ServiceResult();
            if (param == null || param.ConceptId == Guid.Empty || param.ParentId == Guid.Empty)
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }

            if (param.ConceptId == param.ParentId)
            {
                return res.OnError(ErrorCode.Err3003, ErrorMessage.Err3003);
            }

            // TODO: Có cần kiểm soát chặt việc: id 2 concept cần cùng 1 dictionary, id conceptlink phải cùng user với concept?
            // Tạm thời => chỉ update được của dictionary đang đăng nhập
            //var userId = this.ServiceCollection.AuthUtil.GetCurrentUserId();
            var dictionaryId = this.ServiceCollection.AuthUtil.GetCurrentDictionaryId();

            // Lấy dữ liệu (song song)
            var taskChildConcept = _repository.SelectObject<Concept>(new
            {
                concept_id = param.ConceptId,
                dictionary_id = dictionaryId
            });

            var taskParentConcept = _repository.SelectObject<Concept>(new
            {
                concept_id = param.ParentId,
                dictionary_id = dictionaryId
            });

            var taskLinkChildToParent = _repository.SelectObject<ConceptRelationship>(new
            {
                concept_id = param.ConceptId,
                parent_id = param.ParentId
            });

            var taskLinkParentToChild = _repository.SelectObject<ConceptRelationship>(new
            {
                concept_id = param.ParentId,
                parent_id = param.ConceptId
            });

            await Task.WhenAll(taskChildConcept, taskParentConcept, taskLinkChildToParent, taskLinkParentToChild);

            var childConcept = taskChildConcept.Result as Concept;
            var parentConcept = taskParentConcept.Result as Concept;
            var linkChildToParent = taskLinkChildToParent.Result as ConceptRelationship;
            var linkParentToChild = taskLinkParentToChild.Result as ConceptRelationship;

            // Kiểm tra các trường hợp
            if (childConcept == null || parentConcept == null)
            {
                return res.OnError(ErrorCode.Err3005, ErrorMessage.Err3005);
            }

            if (param.ConceptLinkId == null || param.ConceptLinkId == Guid.Empty)
            {
                // TH1: Liên kết = "No link" <=> Xóa bỏ liên kết nếu có.
                // Có hay không có liên kết parent -> child đều xử lý như nhau, do sẽ không tạo circle link.

                if (linkChildToParent != null)
                {
                    _ = await _conceptRelRepository.Delete(new
                    {
                        dictionary_id = dictionaryId,
                        concept_id = param.ConceptId,
                        parent_id = param.ParentId
                    });
                }
            }
            else
            {
                // TH2: Liên kết khác "No link"

                // Nếu có liên kết parent -> child và không cưỡng chế cập nhật => lỗi circle link
                if (linkParentToChild != null)
                {
                    if (param.IsForced != true)
                    {
                        return res.OnError(ErrorCode.Err3004, ErrorMessage.Err3004);
                    }
                    _ = await _conceptRelRepository.Delete(new
                    {
                        dictionary_id = dictionaryId,
                        concept_id = param.ParentId,
                        parent_id = param.ConceptId
                    });
                }

                if (linkChildToParent == null) // nếu có parent -> child thì chắc chắn child -> parent hiện tại đang = null => rơi vào TH này
                {
                    _ = await _conceptRelRepository.Insert(new concept_relationship
                    {
                        dictionary_id = dictionaryId,
                        concept_id = param.ConceptId,
                        parent_id = param.ParentId,
                        concept_link_id = param.ConceptLinkId,
                        created_date = DateTime.Now
                    });
                }
                else if (linkChildToParent.ConceptLinkId != param.ConceptLinkId)
                {
                    _ = await _conceptRelRepository.Update(new
                    {
                        concept_relationship_id = linkChildToParent.ConceptRelationshipId,
                        concept_link_id = param.ConceptLinkId,
                        modified_date = DateTime.Now
                    });
                }
            }


            return res;
        }

        /// <summary>
        /// Thực hiện lấy danh sách gợi ý concept từ những từ khóa người dùng cung cấp
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> GetListRecommendConcept(List<string> keywords, Guid? dictionaryId)
        {
            var res = new ServiceResult();

            if (dictionaryId == null || dictionaryId == Guid.Empty)
            {
                dictionaryId = this.ServiceCollection.AuthUtil.GetCurrentDictionaryId();
            }

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
        /// <summary>
        /// Hàm chạy giải thuật heuristic tìm giá trị activate của nút trong mạng
        /// </summary>
        /// <param name="size"></param>
        /// <param name="linkStrengthArr"></param>
        /// <returns></returns>
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

        #region Tree service
        /// <summary>
        /// Lấy dữ liệu tree của concept
        /// </summary>
        /// <param name="conceptId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> GetTree(Guid conceptId)
        {
            var res = new ServiceResult();
            if (conceptId == Guid.Empty)
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }

            var taskConcept = _repository.SelectObject<Concept>(new
            {
                concept_id = conceptId,
            });

            var taskParentLinkedConcept = _repository.SelectObjects<ViewConceptRelationship>(new
            {
                child_id = conceptId,
            });

            var taskChildLinkedConcept = _repository.SelectObjects<ViewConceptRelationship>(new
            {
                parent_id = conceptId,
            });

            var taskLinkedExample = _repository.SelectObjects<ViewExampleRelationship>(new
            {
                concept_id = conceptId
            });

            var taskExampleLink = _repository.SelectObjects<ExampleLink>(new
            {
                user_id = this.ServiceCollection.AuthUtil.GetCurrentUserId()
            });

            await Task.WhenAll(taskConcept, taskParentLinkedConcept, taskChildLinkedConcept, taskLinkedExample, taskExampleLink);

            var concept = taskConcept.Result as Concept;
            if (concept == null)
            {
                return res.OnError(ErrorCode.Err3005, ErrorMessage.Err3005);
            }

            var listParent = (taskParentLinkedConcept.Result as List<ViewConceptRelationship>)
                .OrderBy(x => x.RelationCreatedDate)
                .Select((x, i) => new
                {
                    ConceptId = x.ParentId,
                    Title = x.ParentName,
                    x.ConceptLinkId,
                    x.ConceptLinkName,
                    SortOrder = i
                }).ToList();

            var listChildren = (taskChildLinkedConcept.Result as List<ViewConceptRelationship>)
                .OrderBy(x => x.RelationCreatedDate)
                .Select((x, i) => new
                {
                    ConceptId = x.ChildId,
                    Title = x.ChildName,
                    x.ConceptLinkId,
                    x.ConceptLinkName,
                    SortOrder = i
                }).ToList();

            var listExample = (taskLinkedExample.Result as List<ViewExampleRelationship>)
                .OrderBy(x => x.RelationCreatedDate)
                .Select((x, i) => new
                {
                    x.ExampleId,
                    x.Example,
                    x.ExampleHtml,
                    x.ExampleLinkId,
                    x.ExampleLinkName,
                    SortOrder = i
                }).ToList();


            var listExampleLink = taskExampleLink.Result as List<ExampleLink>;
            var listTmpCount = listExample.GroupBy(x => x.ExampleLinkId, (key, g) =>
            {
                var item = g.FirstOrDefault();
                return new
                {
                    ExampleLinkId = key,
                    ExampleLinkName = item?.ExampleLinkName,
                    Count = g.Count()
                };
            });
            var listCountExampleLink = from l in listExampleLink
                                       join c in listTmpCount on l.ExampleLinkId equals c.ExampleLinkId into gr
                                       from t in gr.DefaultIfEmpty()
                                       orderby l.SortOrder
                                       select new
                                       {
                                           l.ExampleLinkId,
                                           l.ExampleLinkName,
                                           Count = t?.Count ?? 0,
                                           l.SortOrder
                                       };

            res.Data = new
            {
                Concept = concept,
                ListParent = listParent,
                ListChildren = listChildren,
                ListExample = listExample,
                ListCountExampleLink = listCountExampleLink
            };
            return res;
        }

        /// <summary>
        /// Lấy dữ liệu tree: các concept cha của 1 concept
        /// </summary>
        /// <param name="conceptId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> GetConceptParents(Guid conceptId)
        {
            var res = new ServiceResult();
            if (conceptId == Guid.Empty)
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }

            var taskConcept = _repository.SelectObject<Concept>(new
            {
                concept_id = conceptId,
            });

            var taskParentLinkedConcept = _repository.SelectObjects<ViewConceptRelationship>(new
            {
                child_id = conceptId,
            });

            await Task.WhenAll(taskConcept, taskParentLinkedConcept);

            var concept = taskConcept.Result as Concept;
            if (concept == null)
            {
                return res.OnError(ErrorCode.Err3005, ErrorMessage.Err3005);
            }

            res.Data = (taskParentLinkedConcept.Result as List<ViewConceptRelationship>)
                .OrderBy(x => x.RelationCreatedDate)
                .Select((x, i) => new
                {
                    ConceptId = x.ParentId,
                    Title = x.ParentName,
                    x.ConceptLinkId,
                    x.ConceptLinkName,
                    SortOrder = i
                }).ToList();

            return res;
        }

        /// <summary>
        /// Lấy dữ liệu tree: các concept con của 1 concept
        /// </summary>
        /// <param name="conceptId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> GetConceptChildren(Guid conceptId)
        {
            var res = new ServiceResult();
            if (conceptId == Guid.Empty)
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }

            var taskConcept = _repository.SelectObject<Concept>(new
            {
                concept_id = conceptId,
            });

            var taskChildLinkedConcept = _repository.SelectObjects<ViewConceptRelationship>(new
            {
                parent_id = conceptId,
            });

            await Task.WhenAll(taskConcept, taskChildLinkedConcept);

            var concept = taskConcept.Result as Concept;
            if (concept == null)
            {
                return res.OnError(ErrorCode.Err3005, ErrorMessage.Err3005);
            }

            res.Data = (taskChildLinkedConcept.Result as List<ViewConceptRelationship>)
                .OrderBy(x => x.RelationCreatedDate)
                .Select((x, i) => new
                {
                    ConceptId = x.ChildId,
                    Title = x.ChildName,
                    x.ConceptLinkId,
                    x.ConceptLinkName,
                    SortOrder = i
                }).ToList();

            return res;
        }

        /// <summary>
        /// Lấy dữ liệu tree: danh sách example liên kết với 1 concept theo loại mối quan hệ
        /// </summary>
        /// <param name="conceptId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> GetLinkedExampleByRelationshipType(Guid conceptId, Guid exampleLinkId)
        {
            var res = new ServiceResult();
            if (conceptId == Guid.Empty || exampleLinkId == Guid.Empty)
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }

            var taskConcept = _repository.SelectObject<Concept>(new
            {
                concept_id = conceptId,
            });

            var taskLinkedExample = _repository.SelectObjects<ViewExampleRelationship>(new
            {
                concept_id = conceptId,
                example_link_id = exampleLinkId
            });

            await Task.WhenAll(taskConcept, taskLinkedExample);

            var concept = taskConcept.Result as Concept;
            if (concept == null)
            {
                return res.OnError(ErrorCode.Err3005, ErrorMessage.Err3005);
            }

            res.Data = (taskLinkedExample.Result as List<ViewExampleRelationship>)
                .OrderBy(x => x.RelationCreatedDate)
                .Select((x, i) => new
                {
                    x.ExampleId,
                    x.Example,
                    x.ExampleHtml,
                    x.ExampleLinkId,
                    x.ExampleLinkName,
                    SortOrder = i
                }).ToList();

            return res;
        }
        #endregion
    }
}
