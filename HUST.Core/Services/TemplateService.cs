using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using HUST.Core.Utils.Extensions;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HUST.Core.Services
{
    /// <summary>
    /// Serivce xử lý template xuất khẩu, nhập khẩu
    /// </summary>
    public class TemplateService : BaseService, ITemplateService
    {
        #region Field
        private readonly ICacheSqlUtil _cacheSql;
        private readonly StorageUtil _storage;
        private readonly IDictionaryRepository _repository;
        private readonly IMailService _mailService;
        private readonly IUserConfigService _userConfigService;

        #endregion

        #region Constructor

        public TemplateService(
            ICacheSqlUtil cacheSql,
            StorageUtil storage,
            IDictionaryRepository dictionaryRepository,
            IMailService mailService,
            IUserConfigService userConfigService,
            IHustServiceCollection serviceCollection) : base(serviceCollection)
        {
            _cacheSql = cacheSql;
            _storage = storage;
            _repository = dictionaryRepository;
            _mailService = mailService;
            _userConfigService = userConfigService;
        }
        #endregion

        #region Method
        /// <summary>
        /// Lấy template nhập khẩu dạng byte
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> DowloadTemplateImportDictionary()
        {
            var configData = await this.GetConfigData();
            if (configData == null)
            {
                return null;
            }

            {
                //var downloadUrl = await _storage.GetDownloadUrlAsync(
                //    StoragePath.Import,
                //    TemplateConfig.FileDefaultName.DefaultTemplateProtect);
                //using var client = new WebClient();
                //var content = client.DownloadData(downloadUrl);
                var filePath = Path.Combine(GlobalConfig.ContentRootPath, 
                    ServerStoragePath.Import, 
                    TemplateConfig.FileDefaultName.DefaultTemplateProtect);
                var content = File.ReadAllBytes(filePath);

                using var stream = new MemoryStream(content);

                using var p = new ExcelPackage(stream);
                var sheets = p.Workbook.Worksheets;

                this.SetConfigData(ref sheets, configData);

                p.Workbook.Properties.SetCustomPropertyValue(
                    TemplateConfig.CustomProperty.TokenPropertyName,
                    TemplateConfig.CustomProperty.TokenPropertyValue);
                p.Save();
                return p.GetAsByteArray();
            }
        }

        /// <summary>
        /// Xuất khẩu
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        public async Task<byte[]> ExportDictionary(string userId, string dictionaryId)
        {
            var configData = await this.GetConfigData(userId);
            if (configData == null)
            {
                return null;
            }

            var exportData = await this.GetExportData(dictionaryId);
            if (exportData == null)
            {
                return null;
            }

            {
                //var downloadUrl = await _storage.GetDownloadUrlAsync(
                //    StoragePath.Import,
                //    TemplateConfig.FileDefaultName.DefaultTemplateProtect);
                //using var client = new WebClient();
                //var content = client.DownloadData(downloadUrl);
                var filePath = Path.Combine(GlobalConfig.ContentRootPath, 
                    ServerStoragePath.Import, 
                    TemplateConfig.FileDefaultName.DefaultTemplateProtect);
                var content = File.ReadAllBytes(filePath);

                using var stream = new MemoryStream(content);

                using var p = new ExcelPackage(stream);
                var sheets = p.Workbook.Worksheets;
                this.SetConfigData(ref sheets, configData);
                this.SetExportData(ref sheets, exportData);

                p.Workbook.Properties.SetCustomPropertyValue(
                    TemplateConfig.CustomProperty.TokenPropertyName,
                    TemplateConfig.CustomProperty.TokenPropertyValue);
                p.Save();
                return p.GetAsByteArray();
            }
        }

        /// <summary>
        /// Backup dữ liệu và gửi vào email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> BackupData(string email, string dictionaryId)
        {
            var res = new ServiceResult();

            if (string.IsNullOrEmpty(email))
            {
                email = this.ServiceCollection.AuthUtil.GetCurrentUser()?.Email;
            }

            if (string.IsNullOrEmpty(dictionaryId))
            {
                dictionaryId = this.ServiceCollection.AuthUtil.GetCurrentDictionaryId()?.ToString();
            }

            // Lấy thông tin về từ điển
            var dict = await _repository.SelectObject<Models.DTO.Dictionary>(new Dictionary<string, object>
            {
                { nameof(dictionary.dictionary_id), dictionaryId }
            }) as Models.DTO.Dictionary;

            if (dict == null)
            {
                return res.OnError(ErrorCode.Err2000, ErrorMessage.Err2000);
            }

            var fileByte = await this.ExportDictionary(dict.UserId?.ToString(), dictionaryId);
            using (var stream = new MemoryStream(fileByte))
            {
                var now = DateTime.Now;
                var fileName = this.GetExportFileName(dict.DictionaryName, now);
                var file = new FormFile(stream, 0, fileByte.Length, fileName, fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = FileContentType.Excel,
                };

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = fileName
                };
                file.ContentDisposition = cd.ToString();

                await _mailService.SendEmailBackupData(email, dict.DictionaryName, file, now);
            }
            return res;
        }

        /// <summary>
        /// Nhập khẩu
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<IServiceResult> ImportDictionary(string dictionaryId, IFormFile file)
        {
            var res = new ServiceResult();
            // Validate file
            if (file == null || Path.GetExtension(file.FileName) != FileExtension.Excel2007)
            {
                return res.OnError(ErrorCode.Err9001, ErrorMessage.Err9001);
            }

            // Kiểm tra từ điển tồn tại hay không
            if (string.IsNullOrEmpty(dictionaryId))
            {
                dictionaryId = this.ServiceCollection.AuthUtil.GetCurrentDictionaryId()?.ToString();
            }

            var dict = await _repository.SelectObject<Models.DTO.Dictionary>(new
            {
                dictionary_id = dictionaryId,
                user_id = this.ServiceCollection.AuthUtil.GetCurrentUserId()
            }) as Models.DTO.Dictionary;

            if(dict == null)
            {
                return res.OnError(ErrorCode.Err2000, ErrorMessage.Err2000);
            }

            // Xử lý nhập khẩu
            using var stream = file.OpenReadStream();
            using var p = new ExcelPackage(stream);

            // Validate file token
            var token = p.Workbook.Properties.GetCustomPropertyValue(TemplateConfig.CustomProperty.TokenPropertyName);
            if (token == null || token.ToString() != TemplateConfig.CustomProperty.TokenPropertyValue)
            {
                return res.OnError(ErrorCode.Err9001, ErrorMessage.Err9001);
            }

            // Xử lý file
            res.Data = await this.HandleImportData(p, dict.DictionaryId);
            //res.Data = p.GetAsByteArray();
            return res;
        }

        /// <summary>
        /// Lưu dữ liệu nhập khẩu
        /// </summary>
        /// <param name="importSession"></param>
        /// <returns></returns>
        public async Task<IServiceResult> DoImport(string importSession)
        {
            var res = new ServiceResult();

            // Kiểm tra cache
            var cacheData = await _cacheSql.GetCache(importSession);
            if(string.IsNullOrEmpty(cacheData))
            {
                return res.OnError(ErrorCode.Err9004, ErrorMessage.Err9004);
            }

            var importData = SerializeUtil.DeserializeObject<CacheImportResult>(cacheData);

            // Kiểm tra dictionary
            var dict = await _repository.SelectObject<Dictionary>(new Dictionary<string, object>
            {
                { nameof(dictionary.dictionary_id), importData.DictionaryId },
                { nameof(dictionary.user_id), this.ServiceCollection.AuthUtil.GetCurrentUserId() }
            }) as Dictionary;
            if (dict == null)
            {
                return res.OnError(ErrorCode.Err2000, ErrorMessage.Err2000);
            }

            var lstConcept = this.ServiceCollection.Mapper.Map<List<concept>>(importData.ListConcept);
            var lstConceptRel = this.ServiceCollection.Mapper.Map<List<concept_relationship>>(importData.ListConceptRelationship);
            var lstExample = this.ServiceCollection.Mapper.Map<List<example>>(importData.ListExample);
            var lstExampleRel = this.ServiceCollection.Mapper.Map<List<example_relationship>>(importData.ListExampleRelationship);
            // Transaction save dữ liệu nhập khẩu
            using (var connection = await _repository.CreateConnectionAsync())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    var result = true;

                    // Xóa trắng data từ điển
                    result = await _repository.DeleteDictionaryData(importData.DictionaryId, transaction);

                    if (result && (lstConcept != null && lstConcept.Count > 0))
                    {
                        result = await _repository.Insert<concept>(lstConcept, transaction);
                    }

                    if (result && (lstExample != null && lstExample.Count > 0))
                    {
                        // Thực tế, có thể làm song song với insert concept
                        result = await _repository.Insert<example>(lstExample, transaction); 
                    }

                    if (result && (lstConceptRel != null && lstConceptRel.Count > 0))
                    {
                        result = await _repository.Insert<concept_relationship>(lstConceptRel, transaction);
                    }

                    if (result && (lstExampleRel != null && lstExampleRel.Count > 0))
                    {
                        // Thực tế, có thể làm song song với insert concept_relationship
                        result = await _repository.Insert<example_relationship>(lstExampleRel, transaction);
                    }

                    if (result)
                    {
                        result = await _cacheSql.DeleteCache(importSession, transaction);
                    }

                    if (result)
                    {
                        transaction.Commit();
                        res.OnSuccess(importData.NumberValidRecord);
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
        #endregion

        #region Helper
        /// <summary>
        /// Lấy tên file export
        /// </summary>
        /// <param name="dictionaryName"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public string GetExportFileName(string dictionaryName, DateTime? dateTime = null)
        {
            var normalizedDictName = (dictionaryName ?? "").Replace(' ', '_');
            if (normalizedDictName.Length > 20)
            {
                normalizedDictName = normalizedDictName.Substring(0, 20);
            }
            var fileName = string.Format(TemplateConfig.FileDefaultName.ExportFile,
                normalizedDictName,
                (dateTime ?? DateTime.Now).ToString("yyyyMMdd'T'HHmmss"));
            return fileName;
        }

        /// <summary>
        /// Lấy dữ liệu sheet config để bind vào mẫu nhập khẩu
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, IEnumerable<string>>> GetConfigData(string userId = null)
        {
            var rawConfigData = await _userConfigService.GetAllConfigData(userId);

            // Kết quả trả về
            var res = new Dictionary<string, IEnumerable<string>>();
            res.Add(nameof(concept_link),
                rawConfigData.ListConceptLink.Where(x => x.concept_link_type != (int)ConceptLinkType.NoLink)?.Select(x => x.concept_link_name));
            res.Add(nameof(example_link),
                rawConfigData.ListExampleLink.Where(x => x.example_link_type != (int)ExampleLinkType.NoLink)?.Select(x => x.example_link_name));
            res.Add(nameof(tone), rawConfigData.ListTone.Select(x => x.tone_name));
            res.Add(nameof(mode), rawConfigData.ListMode.Select(x => x.mode_name));
            res.Add(nameof(register), rawConfigData.ListRegister.Select(x => x.register_name));
            res.Add(nameof(nuance), rawConfigData.ListNuance.Select(x => x.nuance_name));
            res.Add(nameof(dialect), rawConfigData.ListDialect.Select(x => x.dialect_name));
            return res;
        }

        /// <summary>
        /// Bind dữ liệu vào sheet Config cho mẫu
        /// </summary>
        /// <param name="sheets"></param>
        /// <param name="configData"></param>
        public void SetConfigData(ref ExcelWorksheets sheets, Dictionary<string, IEnumerable<string>> configData)
        {
            var ws = sheets[TemplateConfig.WorksheetName.Config];
            var startRow = TemplateConfig.StartRowData;

            var lstConceptLink = configData[nameof(concept_link)] ?? new List<string>();
            var lstExampleLink = configData[nameof(example_link)] ?? new List<string>();
            var lstTone = configData[nameof(tone)] ?? new List<string>();
            var lstMode = configData[nameof(mode)] ?? new List<string>();
            var lstRegister = configData[nameof(register)] ?? new List<string>();
            var lstNuance = configData[nameof(nuance)] ?? new List<string>();
            var lstDialect = configData[nameof(dialect)] ?? new List<string>();

            ws.Cells[startRow, TemplateConfig.ConfigSheet.ConceptLink].LoadFromCollection(lstConceptLink);
            ws.Cells[startRow, TemplateConfig.ConfigSheet.ExampleLink].LoadFromCollection(lstExampleLink);
            ws.Cells[startRow, TemplateConfig.ConfigSheet.Tone].LoadFromCollection(lstTone);
            ws.Cells[startRow, TemplateConfig.ConfigSheet.Mode].LoadFromCollection(lstMode);
            ws.Cells[startRow, TemplateConfig.ConfigSheet.Register].LoadFromCollection(lstRegister);
            ws.Cells[startRow, TemplateConfig.ConfigSheet.Nuance].LoadFromCollection(lstNuance);
            ws.Cells[startRow, TemplateConfig.ConfigSheet.Dialect].LoadFromCollection(lstDialect);

        }

        /// <summary>
        /// Lấy dữ liệu xuất khẩu
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> GetExportData(string dictionaryId)
        {
            if (string.IsNullOrEmpty(dictionaryId))
            {
                dictionaryId = this.ServiceCollection.AuthUtil.GetCurrentDictionaryId()?.ToString();
            }

            var tables = new string[]
            {
                nameof(concept),
                nameof(view_concept_relationship),
                nameof(view_example),
                nameof(view_example_relationship)
            };

            var param = new Dictionary<string, Dictionary<string, object>>()
            {
                {
                    nameof(concept),
                    new Dictionary<string, object> { { nameof(concept.dictionary_id), dictionaryId } }
                },
                {
                    nameof(view_concept_relationship),
                    new Dictionary<string, object> { { nameof(view_concept_relationship.dictionary_id), dictionaryId } }
                },
                {
                    nameof(view_example),
                    new Dictionary<string, object> { { nameof(view_example.dictionary_id), dictionaryId } }
                },
                {
                    nameof(view_example_relationship),
                    new Dictionary<string, object> { { nameof(view_example_relationship.dictionary_id), dictionaryId } }
                }
            };

            var queryRes = await _repository.SelectManyObjects(tables, param) as Dictionary<string, object>;

            if (queryRes == null)
            {
                return null;
            }
            return queryRes;
        }

        /// <summary>
        /// Bind dữ liệu vào sheet dữ liệu
        /// </summary>
        /// <param name="sheets"></param>
        /// <param name="exportData"></param>
        public void SetExportData(ref ExcelWorksheets sheets, Dictionary<string, object> exportData)
        {
            var startRow = TemplateConfig.StartRowData;
            var startCol = TemplateConfig.StartColData;

            var lstConcept = exportData[nameof(concept)] as List<concept> ?? new List<concept>();
            var lstConceptRel = exportData[nameof(view_concept_relationship)] as List<view_concept_relationship> ?? new List<view_concept_relationship>();
            var lstExample = exportData[nameof(view_example)] as List<view_example> ?? new List<view_example>();
            var lstExampleRel = exportData[nameof(view_example_relationship)] as List<view_example_relationship> ?? new List<view_example_relationship>();

            // Bind vào sheet concept
            var ws = sheets[TemplateConfig.WorksheetName.Concept]; 
            var lstConceptImport = this.ServiceCollection.Mapper.Map<List<ConceptImport>>(lstConcept);
            ws.Cells[startRow, startCol].LoadFromCollection(lstConceptImport);
            
            // Bind vào sheet concept relationship
            ws = sheets[TemplateConfig.WorksheetName.ConceptRelationship];
            var lstConceptRelImport = this.ServiceCollection.Mapper.Map<List<ConceptRelationshipImport>>(lstConceptRel);
            ws.Cells[startRow, startCol].LoadFromCollection(lstConceptRelImport);

            // Bind vào sheet example
            ws = sheets[TemplateConfig.WorksheetName.Example];
            var lstExampleImport = this.ServiceCollection.Mapper.Map<List<ExampleImport>>(lstExample);
            ws.Cells[startRow, startCol].LoadFromCollection(lstExampleImport);

            // Bind vào sheet example relationship
            ws = sheets[TemplateConfig.WorksheetName.ExampleRelationship];
            var lstExampleRelImport = this.ServiceCollection.Mapper.Map<List<ExampleRelationshipImport>>(lstExampleRel);
            ws.Cells[startRow, startCol].LoadFromCollection(lstExampleRelImport);

        }

        /// <summary>
        /// Xử lý dữ liệu nhập khẩu
        /// </summary>
        /// <param name="wb"></param>
        /// <returns></returns>
        public async Task<TempImportResult> HandleImportData(ExcelPackage p, Guid dictionaryId)
        {
            var sheets = p.Workbook.Worksheets;
            var now = DateTime.Now;

            var configData = await _userConfigService.GetAllConfigData(); // Lấy dữ liệu config của user hiện tại
            var defaultTone = configData.ListTone.Find(x => x.tone_type == (int)ToneType.Neutral);
            var defaultMode = configData.ListMode.Find(x => x.mode_type == (int)ModeType.Neutral);
            var defaultRegister = configData.ListRegister.Find(x => x.register_type == (int)RegisterType.Neutral);
            var defaultNuance = configData.ListNuance.Find(x => x.nuance_type == (int)NuanceType.Neutral);
            var defaultDialect = configData.ListDialect.Find(x => x.dialect_type == (int)DialectType.Neutral);

            var lstValidateError = new List<ValidateResultImport>();

            // Dữ liệu concept
            var ws = sheets[TemplateConfig.WorksheetName.Concept];
            var errorColIndex = TemplateConfig.ConceptSheet.Error;
            this.ClearError(ref ws, errorColIndex);

            var lstConcept = new List<Concept>();
            var conceptRows = ws.ConvertSheetToObjects<ConceptImport>().ToList();
            foreach(var row in conceptRows)
            {
                // Validate
                var validateRes = row.ValidateBusinessMater(lstConcept);

                if(validateRes.IsValid != true)
                {
                    validateRes.SheetIndex = ws.Index;
                    validateRes.SheetName = ws.Name;
                    this.SetErrorMessage(ref ws, row.RowIndex, errorColIndex, validateRes.ErrorMessage);
                    lstValidateError.Add(validateRes);
                    continue;
                }

                lstConcept.Add(new Concept
                {
                    ConceptId = Guid.NewGuid(),
                    Title = row.Title,
                    Description = row.Description,
                    DictionaryId = dictionaryId,
                    CreatedDate = now
                });
            }

            // Dữ liệu concept relationship
            ws = sheets[TemplateConfig.WorksheetName.ConceptRelationship];
            errorColIndex = TemplateConfig.ConceptRelationshipSheet.Error;
            this.ClearError(ref ws, errorColIndex);

            var lstConceptRel = new List<ConceptRelationship>();
            var conceptRelRows = ws.ConvertSheetToObjects<ConceptRelationshipImport>().ToList();
            foreach(var row in conceptRelRows)
            {
                // Validate
                var findChildConcept = lstConcept.Find(x => x.Title == row.ChildName);
                var findParentConcept = lstConcept.Find(x => x.Title == row.ParentName);
                var findRelation = configData.ListConceptLink.Find(x => x.concept_link_name == row.ConceptLinkName);

                var validateRes = row.ValidateBusinessMater(lstConceptRel, findChildConcept, findParentConcept, findRelation);

                if (validateRes.IsValid != true)
                {
                    validateRes.SheetIndex = ws.Index;
                    validateRes.SheetName = ws.Name;
                    this.SetErrorMessage(ref ws, row.RowIndex, errorColIndex, validateRes.ErrorMessage);
                    lstValidateError.Add(validateRes);
                    continue;
                }

                lstConceptRel.Add(new ConceptRelationship
                {
                    ConceptId = findChildConcept.ConceptId,
                    ParentId = findParentConcept.ConceptId,
                    ConceptLinkId = findRelation.concept_link_id,
                    DictionaryId = dictionaryId,
                    CreatedDate = now
                });
            }

            // Dữ liệu example
            ws = sheets[TemplateConfig.WorksheetName.Example];
            errorColIndex = TemplateConfig.ExampleSheet.Error;
            this.ClearError(ref ws, errorColIndex);

            var lstExample = new List<Example>();
            var exampleRows = ws.ConvertSheetToObjects<ExampleImport>().ToList();
            foreach (var row in exampleRows)
            {
                // Validate
                var findTone = configData.ListTone.Find(x => x.tone_name == row.ToneName);
                var findMode = configData.ListMode.Find(x => x.mode_name == row.ModeName);
                var findRegister = configData.ListRegister.Find(x => x.register_name == row.RegisterName);
                var findNuance = configData.ListNuance.Find(x => x.nuance_name == row.NuanceName);
                var findDialect = configData.ListDialect.Find(x => x.dialect_name == row.DialectName);

                var validateRes = row.ValidateBusinessMater(lstExample, findTone, findMode, findRegister, findNuance, findDialect);

                if (validateRes.IsValid != true)
                {
                    validateRes.SheetIndex = ws.Index;
                    validateRes.SheetName = ws.Name;
                    this.SetErrorMessage(ref ws, row.RowIndex, errorColIndex, validateRes.ErrorMessage);
                    lstValidateError.Add(validateRes);
                    continue;
                }

                // Nếu giá trị lựa chọn ở dạng default (neutral) thì khi lưu db để là null
                //var toneId = (string.IsNullOrEmpty(row.ToneName) || row.ToneName == UserConfigDataDefault.ToneDefault) ? (Guid?)null : findTone.tone_id;
                //var modeId = (string.IsNullOrEmpty(row.ModeName) || row.ModeName == UserConfigDataDefault.ModeDefault) ? (Guid?)null : findMode.mode_id;
                //var registerId = (string.IsNullOrEmpty(row.RegisterName) || row.RegisterName == UserConfigDataDefault.RegisterDefault) ? (Guid?)null : findRegister.register_id;
                //var nuanceId = (string.IsNullOrEmpty(row.NuanceName) || row.NuanceName == UserConfigDataDefault.NuanceDefault) ? (Guid?)null : findNuance.nuance_id;
                //var dialectId = (string.IsNullOrEmpty(row.DialectName) || row.DialectName == UserConfigDataDefault.DialectDefault) ? (Guid?)null : findDialect.dialect_id;

                // Nếu giá trị tone, mode... để trống => mặc định lấy default
                var toneId = string.IsNullOrEmpty(row.ToneName) ? defaultTone?.tone_id : findTone?.tone_id;
                var modeId = string.IsNullOrEmpty(row.ModeName) ? defaultMode?.mode_id : findMode?.mode_id;
                var registerId = string.IsNullOrEmpty(row.RegisterName) ? defaultRegister?.register_id : findRegister?.register_id;
                var nuanceId = string.IsNullOrEmpty(row.NuanceName) ? defaultNuance?.nuance_id : findNuance?.nuance_id;
                var dialectId = string.IsNullOrEmpty(row.DialectName) ? defaultDialect?.dialect_id : findDialect?.dialect_id;

                lstExample.Add(new Example
                {
                    ExampleId = Guid.NewGuid(),
                    Detail = FunctionUtil.StripHtml(row.DetailHtml)?.Trim(),
                    DetailHtml = FunctionUtil.StripHtmlExceptHightlight(row.DetailHtml)?.Trim(),
                    ToneId = toneId,
                    ModeId = modeId,
                    RegisterId = registerId,
                    NuanceId = nuanceId,
                    DialectId = dialectId,
                    Note = row.Note,
                    DictionaryId = dictionaryId,
                    CreatedDate = now
                });
            }

            // Dữ liệu example relationship
            ws = sheets[TemplateConfig.WorksheetName.ExampleRelationship];
            errorColIndex = TemplateConfig.ExampleRelationshipSheet.Error;
            this.ClearError(ref ws, errorColIndex);

            var lstExampleRel = new List<ExampleRelationship>();
            var exampleRelRows = ws.ConvertSheetToObjects<ExampleRelationshipImport>().ToList();
            foreach (var row in exampleRelRows)
            {
                // Validate
                var findConcept = lstConcept.Find(x => x.Title == row.Concept);
                var findExample = lstExample.Find(x => x.DetailHtml == row.ExampleHtml);
                var findRelation = configData.ListExampleLink.Find(x => x.example_link_name == row.ExampleLinkName);

                var validateRes = row.ValidateBusinessMater(lstExampleRel, findConcept, findExample, findRelation);

                if (validateRes.IsValid != true)
                {
                    validateRes.SheetIndex = ws.Index;
                    validateRes.SheetName = ws.Name;
                    this.SetErrorMessage(ref ws, row.RowIndex, errorColIndex, validateRes.ErrorMessage);
                    lstValidateError.Add(validateRes);
                    continue;
                }

                lstExampleRel.Add(new ExampleRelationship
                {
                    ConceptId = findConcept.ConceptId,
                    ExampleId = findExample.ExampleId,
                    ExampleLinkId = findRelation.example_link_id,
                    DictionaryId = dictionaryId,
                    CreatedDate = now
                });
            }

            // Xử lý thêm với dữ liệu example
            // Thêm hightlight mặc định
            // Modified by pthieu 09.06.2023: Nếu không có hight light thì mặc định toàn bộ từ là hight light
            Parallel.ForEach(lstExample, ex => {
                if (!string.IsNullOrEmpty(ex.DetailHtml) && !FunctionUtil.CheckStringHasHightlight(ex.DetailHtml))
                {
                    ex.DetailHtml = FunctionUtil.GenerateStringHightlight(ex.DetailHtml);
                }
            });
            

            // Lưu dữ liệu hợp lệ vào cache sql
            var numberValidRecord = lstConcept.Count + lstConceptRel.Count + lstExample.Count + lstExampleRel.Count;
            var importSession = $"{Guid.NewGuid()}_{dictionaryId}";
            if (numberValidRecord > 0)
            {
                var cacheData = new CacheImportResult
                {
                    DictionaryId = dictionaryId,
                    NumberValidRecord = numberValidRecord,
                    ListConcept = lstConcept,
                    ListExample = lstExample,
                    ListConceptRelationship = lstConceptRel,
                    ListExampleRelationship = lstExampleRel
                };

                // Xóa cache import cũ
                await _cacheSql.DeleteCache(new 
                { 
                    user_id = this.ServiceCollection.AuthUtil.GetCurrentUserId(),
                    cache_type = (int)CacheSqlType.ImportDictionary
                });

                // Thêm cache mới
                await _cacheSql.SetCache(importSession, SerializeUtil.SerializeObject(cacheData), (int)CacheSqlType.ImportDictionary);
            }
            
            var res = new TempImportResult
            {
                ImportSession = importSession,
                NumberValid = numberValidRecord,
                NumberError = lstValidateError.Count,
                ListValidateError = lstValidateError,
                StrFileError = lstValidateError.Count > 0 ? Convert.ToBase64String(p.GetAsByteArray()) : null
            };

            return res;
        }

        /// <summary>
        /// Xóa bỏ style, giá trị lỗi của sheet (reset)
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="errorColIndex"></param>
        public void ClearError(ref ExcelWorksheet ws, int errorColIndex) {
            var errorColName = FunctionUtil.GetExcelColumnName(errorColIndex);
            ws.Cells.Style.Font.Color.SetColor(Color.Black);
            ws.Cells[$"{errorColName}:{errorColName}"].Clear();
        }

        /// <summary>
        /// Thêm thông báo lỗi validate dòng nhập khẩu
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="rowIndex"></param>
        /// <param name="errorColIndex"></param>
        /// <param name="errorMessage"></param>
        public void SetErrorMessage(ref ExcelWorksheet ws, int rowIndex, int errorColIndex, string errorMessage)
        {
            ws.Row(rowIndex).Style.Font.Color.SetColor(Color.Red);
            ws.Cells[rowIndex, errorColIndex].Value = errorMessage;
        }
        #endregion
    }
}
