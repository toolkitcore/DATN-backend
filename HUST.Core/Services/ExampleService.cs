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
    /// Serivce xử lý example
    /// </summary>
    public class ExampleService : BaseService, IExampleService
    {
        #region Field

        private readonly IExampleRepository _repository;
        private readonly IUserConfigService _userConfigService;

        #endregion

        #region Constructor

        public ExampleService(IExampleRepository repository,
            IUserConfigService userConfigService,
            IHustServiceCollection serviceCollection) : base(serviceCollection)
        {
            _repository = repository;
            _userConfigService = userConfigService;
        }



        #endregion

        #region Method

        /// <summary>
        /// Thêm 1 example vào từ điển
        /// </summary>
        /// <param name="example"></param>
        /// <returns></returns>
        public async Task<IServiceResult> AddExample(Example example)
        {
            var res = new ServiceResult();

            // Check đầu vào
            if (example == null || string.IsNullOrWhiteSpace(example.DetailHtml))
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }

            // PTHIEU 02.07.2023: Kiểm tra hightlight part, loại bỏ tag html trừ tag hightlight
            example.DetailHtml = FunctionUtil.StripHtmlExceptHightlight(example.DetailHtml)?.Trim();
            example.Detail = FunctionUtil.StripHtml(example.DetailHtml)?.Trim();

            // Kiểm tra hightlight part
            if (!FunctionUtil.CheckStringHasHightlight(example.DetailHtml) || string.IsNullOrEmpty(example.Detail))
            {
                return res.OnError(ErrorCode.Err4002, ErrorMessage.Err4002);
            }

            // Gán dictId mặc định nếu chưa có
            if (example.DictionaryId == null || example.DictionaryId == Guid.Empty)
            {
                example.DictionaryId = this.ServiceCollection.AuthUtil.GetCurrentDictionaryId();
            }

            // Check trùng
            var existExample = await _repository.SelectObject<Example>(new Dictionary<string, object>
            {
                { nameof(Models.Entity.example.dictionary_id), example.DictionaryId },
                { nameof(Models.Entity.example.detail_html), example.DetailHtml }
            }) as Example;

            if (existExample != null)
            {
                return res.OnError(ErrorCode.Err4001, ErrorMessage.Err4001);
            }

            // Chuẩn bị dữ liệu đem đi lưu
            // TODO: Có thể xử lý chặt hơn
            // - Validate tone_id, mode_id... có tồn tại trong config data hay không
            // - Validate liên kết:
            // + concept có tồn tại trong cùng dictionary không
            // + example_link_id có tồn tại hay không
            // + liên kết có bị trùng hay không (gtrùng hoàn toàn, trùng concept)

            // Xử lý example
            var now = DateTime.Now;
            var configData = await _userConfigService.GetAllConfigData(); // Lấy dữ liệu config của user hiện tại
            var defaultTone = configData.ListTone.Find(x => x.tone_type == (int)ToneType.Neutral);
            var defaultMode = configData.ListMode.Find(x => x.mode_type == (int)ModeType.Neutral);
            var defaultRegister = configData.ListRegister.Find(x => x.register_type == (int)RegisterType.Neutral);
            var defaultNuance = configData.ListNuance.Find(x => x.nuance_type == (int)NuanceType.Neutral);
            var defaultDialect = configData.ListDialect.Find(x => x.dialect_type == (int)DialectType.Neutral);

            var exampleEntity = this.ServiceCollection.Mapper.Map<Models.Entity.example>(example);
            exampleEntity.example_id = Guid.NewGuid();
            exampleEntity.created_date = now;
            exampleEntity.tone_id ??= defaultTone.tone_id;
            exampleEntity.mode_id ??= defaultMode.mode_id;
            exampleEntity.register_id ??= defaultRegister.register_id;
            exampleEntity.nuance_id ??= defaultNuance.nuance_id;
            exampleEntity.dialect_id ??= defaultDialect.dialect_id;

            // Xử lý liên kết
            var mapLstRel = new List<example_relationship>();

            if(example.ListExampleRelationship != null && example.ListExampleRelationship.Count > 0)
            {
                var lstViewRel = example.ListExampleRelationship
                    .Where(x =>
                    {
                        return (x.ConceptId != null && x.ConceptId != Guid.Empty && x.ExampleLinkId != null && x.ExampleLinkId != Guid.Empty);
                    })
                    .Distinct(); // Loại bỏ việc trùng hoàn toàn, chỉ giữ lại 1 bản ghi

                // Trường hợp vẫn tồn tại 2 bản ghi có concept trùng nhau
                // <=> có 2 liên kết cùng tới 1 concept, nhưng loại liên kết khác nhau
                var lstLinkedConcept = lstViewRel.Select(x => x.ConceptId);
                if (lstLinkedConcept.Count() != lstLinkedConcept.Distinct().Count())
                {
                    return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
                }

                var lstRel = lstViewRel.Select(x => new ExampleRelationship
                {
                    ConceptId = x.ConceptId,
                    ExampleLinkId = x.ExampleLinkId,
                    ExampleId = exampleEntity.example_id,
                    DictionaryId = exampleEntity.dictionary_id,
                    CreatedDate = now,
                });

                mapLstRel = this.ServiceCollection.Mapper.Map<List<example_relationship>>(lstRel);
            }

            using (var conn = await _repository.CreateConnectionAsync())
            {
                using (var transaction = conn.BeginTransaction())
                {
                    var result = true;

                    result = await _repository.Insert(exampleEntity, transaction);

                    if (result && mapLstRel.Count > 0)
                    {
                        result = await _repository.Insert<example_relationship>(mapLstRel, transaction);
                    }

                    if (result)
                    {
                        transaction.Commit();
                        res.OnSuccess();
                    }
                    else
                    {
                        transaction.Rollback();
                        res.OnError(ErrorCode.Err9999);
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Thực hiện cập nhật example
        /// </summary>
        /// <param name="example"></param>
        /// <returns></returns>
        public async Task<IServiceResult> UpdateExample(Example example)
        {
            var res = new ServiceResult();

            // Check đầu vào
            if (example == null 
                || example.ExampleId == Guid.Empty
                || example.DictionaryId == null
                || example.DictionaryId == Guid.Empty
                || string.IsNullOrWhiteSpace(example.DetailHtml))
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }

            // PTHIEU 02.07.2023: Kiểm tra hightlight part, loại bỏ tag html trừ tag hightlight
            example.DetailHtml = FunctionUtil.StripHtmlExceptHightlight(example.DetailHtml)?.Trim();
            example.Detail = FunctionUtil.StripHtml(example.DetailHtml)?.Trim();

            // Kiểm tra hightlight part
            if (!FunctionUtil.CheckStringHasHightlight(example.DetailHtml) || string.IsNullOrEmpty(example.Detail))
            {
                return res.OnError(ErrorCode.Err4002, ErrorMessage.Err4002);
            }

            // Lấy ra bản ghi đã lưu trong db
            var savedExample = await _repository.SelectObject<Example>(new Dictionary<string, object>
            {
                { nameof(Models.Entity.example.example_id), example.ExampleId },
                { nameof(Models.Entity.example.dictionary_id), example.DictionaryId },
            }) as Example;

            if (savedExample == null)
            {
                return res.OnError(ErrorCode.Err4000, ErrorMessage.Err4000);
            } 
            else if (savedExample.ModifiedDate != example.ModifiedDate) // Kiểm tra dữ liệu bị cũ
            {
                return res.OnError(ErrorCode.Err9998, ErrorMessage.Err9998);
            }

            // Check trùng
            // Có 1 cách khác để check trùng là select bản ghi có detail_html tương ứng, kiểm tra id != example_id hiện tại
            if (example.DetailHtml != savedExample.DetailHtml)
            {
                // Kiểm tra example đã tồn tại
                var existExample = await _repository.SelectObject<Example>(new Dictionary<string, object>
                {
                    { nameof(Models.Entity.example.dictionary_id), example.DictionaryId },
                    { nameof(Models.Entity.example.detail_html), example.DetailHtml }
                }) as Example;

                if (existExample != null)
                {
                    return res.OnError(ErrorCode.Err4001, ErrorMessage.Err4001);
                }
            }

            // Chuẩn bị dữ liệu đem đi lưu
            // TODO: Có thể xử lý validate chặt hơn

            // Xử lý example: gán lại 1 số trường lấy từ savedExample, xử lý 1 số trường
            var now = DateTime.Now;
            var configData = await _userConfigService.GetAllConfigData(); // Lấy dữ liệu config của user hiện tại
            var defaultTone = configData.ListTone.Find(x => x.tone_type == (int)ToneType.Neutral);
            var defaultMode = configData.ListMode.Find(x => x.mode_type == (int)ModeType.Neutral);
            var defaultRegister = configData.ListRegister.Find(x => x.register_type == (int)RegisterType.Neutral);
            var defaultNuance = configData.ListNuance.Find(x => x.nuance_type == (int)NuanceType.Neutral);
            var defaultDialect = configData.ListDialect.Find(x => x.dialect_type == (int)DialectType.Neutral);

            var exampleEntity = this.ServiceCollection.Mapper.Map<Models.Entity.example>(example);
            exampleEntity.created_date = savedExample.CreatedDate;
            exampleEntity.modified_date = now; // cập nhật modified date
            exampleEntity.tone_id ??= defaultTone.tone_id;
            exampleEntity.mode_id ??= defaultMode.mode_id;
            exampleEntity.register_id ??= defaultRegister.register_id;
            exampleEntity.nuance_id ??= defaultNuance.nuance_id;
            exampleEntity.dialect_id ??= defaultDialect.dialect_id;

            // Xử lý liên kết (tương tự khi thêm mới example)
            var mapLstRel = new List<example_relationship>();

            if (example.ListExampleRelationship != null && example.ListExampleRelationship.Count > 0)
            {
                var lstViewRel = example.ListExampleRelationship
                    .Where(x =>
                    {
                        return (x.ConceptId != null && x.ConceptId != Guid.Empty && x.ExampleLinkId != null && x.ExampleLinkId != Guid.Empty);
                    })
                    .Distinct(); // Loại bỏ việc trùng hoàn toàn, chỉ giữ lại 1 bản ghi

                // Trường hợp vẫn tồn tại 2 bản ghi có concept trùng nhau
                // <=> có 2 liên kết cùng tới 1 concept, nhưng loại liên kết khác nhau
                var lstLinkedConcept = lstViewRel.Select(x => x.ConceptId);
                if (lstLinkedConcept.Count() != lstLinkedConcept.Distinct().Count())
                {
                    return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
                }

                var lstRel = lstViewRel.Select(x => new ExampleRelationship
                {
                    ConceptId = x.ConceptId,
                    ExampleLinkId = x.ExampleLinkId,
                    ExampleId = exampleEntity.example_id,
                    DictionaryId = exampleEntity.dictionary_id,
                    CreatedDate = now,
                });

                mapLstRel = this.ServiceCollection.Mapper.Map<List<example_relationship>>(lstRel);
            }

            using (var conn = await _repository.CreateConnectionAsync())
            {
                using (var transaction = conn.BeginTransaction())
                {
                    var result = true;

                    result = await _repository.Update(exampleEntity, transaction);

                    if (result)
                    {
                        // Vì chưa chắc trước đó có liên kết, nên có thể kết quả xóa trả về false (do không xóa được bản ghi nào)
                        // Cách khác là: select liên kết trước khi xóa
                        _ = await _repository.Delete<example_relationship>(new 
                        { 
                            example_id = example.ExampleId
                        }, transaction);
                    }

                    if (result && mapLstRel.Count > 0)
                    {
                        result = await _repository.Insert<example_relationship>(mapLstRel, transaction);
                    }

                    if (result)
                    {
                        transaction.Commit();
                        res.OnSuccess();
                    }
                    else
                    {
                        transaction.Rollback();
                        res.OnError(ErrorCode.Err9999);
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Thực hiện xóa example
        /// </summary>
        /// <param name="exampleId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> DeleteExample(Guid exampleId)
        {
            var res = new ServiceResult();

            if (exampleId == Guid.Empty)
            {
                return res.OnError(ErrorCode.Err9999);
            }

            // Thực hiện xóa
            var result = await _repository.Delete(new
            {
                example_id = exampleId
            });

            if (!result)
            {
                return res.OnError(ErrorCode.Err4000, ErrorMessage.Err4000);
            }

            return res;
        }

        /// <summary>
        /// Lấy dữ liệu example
        /// </summary>
        /// <param name="exampleId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> GetExample(Guid exampleId)
        {
            var res = new ServiceResult();

            if (exampleId == Guid.Empty)
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }

            // Lấy dữ liệu
            var tables = new string[]
            {
                nameof(Models.Entity.example),
                nameof(view_example),
                nameof(view_example_relationship)
            };

            var param = new Dictionary<string, Dictionary<string, object>>()
            {
                {
                    nameof(Models.Entity.example),
                    new Dictionary<string, object> { { nameof(Models.Entity.example.example_id), exampleId } }
                },
                {
                    nameof(view_example),
                    new Dictionary<string, object> { { nameof(view_example.example_id), exampleId } }
                },
                {
                    nameof(view_example_relationship),
                    new Dictionary<string, object> { { nameof(view_example_relationship.example_id), exampleId } }
                }
            };

            var queryRes = await _repository.SelectManyObjects(tables, param) as Dictionary<string, object>;
            if (queryRes == null)
            {
                return res.OnSuccess(null);
            }
            var example = (queryRes[nameof(Models.Entity.example)] as List<example>)?.FirstOrDefault();
            var viewExample = (queryRes[nameof(view_example)] as List<view_example>)?.FirstOrDefault();
            var lstViewExampleRel = queryRes[nameof(view_example_relationship)] as List<view_example_relationship>;

            if(example == null || viewExample == null)
            {
                return res.OnError(ErrorCode.Err4000, ErrorMessage.Err4000);
            }

            // Map dữ liệu
            var mapExample = this.ServiceCollection.Mapper.Map<Example>(example);
            mapExample.ToneName = viewExample.tone_name;
            mapExample.ModeName = viewExample.mode_name;
            mapExample.RegisterName = viewExample.register_name;
            mapExample.NuanceName = viewExample.nuance_name;
            mapExample.DialectName = viewExample.dialect_name;
            mapExample.ListExampleRelationship = this.ServiceCollection.Mapper.Map<List<ViewExampleRelationship>>(lstViewExampleRel);
            
            return res.OnSuccess(mapExample);
        }

        /// <summary>
        /// Tìm kiếm example
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<IServiceResult> SearchExample(SearchExampleParam param)
        {
            var res = new ServiceResult();
            if (param.DictionaryId == null || param.DictionaryId == Guid.Empty)
            {
                param.DictionaryId = this.ServiceCollection.AuthUtil.GetCurrentDictionaryId();
            }

            var data = await _repository.SearchExample(param);

            // TODO: Xem xét rule sắp xếp (theo created date?)
            res.Data = data.Select(x => new
            {
                x.ExampleId,
                x.Detail,
                x.DetailHtml
            });

            return res;
        }

        #endregion



    }
}
