using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.Entity;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HUST.Core.Services
{
    /// <summary>
    /// Serivce xử lý template xuất khẩu, nhập khẩu
    /// </summary>
    public class TemplateService : BaseService, ITemplateService
    {
        #region Field

        private readonly IDictionaryRepository _repository;
        private readonly StorageUtil _storage;
        private readonly IMailService _mailService;

        #endregion

        #region Constructor

        public TemplateService(IDictionaryRepository dictionaryRepository,
            StorageUtil storage,
            IMailService mailService,
            IHustServiceCollection serviceCollection) : base(serviceCollection)
        {
            _repository = dictionaryRepository;
            _storage = storage;
            _mailService = mailService;
        }
        #endregion

        #region Method
        /// <summary>
        /// Lấy template nhập khẩu dạng byte
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> DowloadTemplateImportDictionary()
        {
            var downloadUrl = await _storage.GetDownloadUrlAsync(
                StoragePath.Import,
                TemplateConfig.FileDefaultName.DefaultTemplateProtect);

            var configData = await this.GetConfigData();
            if (configData == null)
            {
                return null;
            }

            {
                using var client = new WebClient();
                var content = client.DownloadData(downloadUrl);

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
            var downloadUrl = await _storage.GetDownloadUrlAsync(
                StoragePath.Import,
                TemplateConfig.FileDefaultName.DefaultTemplateProtect);

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
                using var client = new WebClient();
                var content = client.DownloadData(downloadUrl);

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

            if(string.IsNullOrEmpty(email))
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

            if(dict == null)
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
            if (string.IsNullOrEmpty(userId))
            {
                userId = this.ServiceCollection.AuthUtil.GetCurrentUserId()?.ToString();
            }

            var tables = new string[]
            {
                nameof(concept_link),
                nameof(example_link),
                nameof(tone),
                nameof(mode),
                nameof(register),
                nameof(nuance),
                nameof(dialect)
            };

            // Không dùng từ "params"
            var param = new Dictionary<string, Dictionary<string, object>>()
            {
                {
                    nameof(concept_link),
                    new Dictionary<string, object> { { nameof(concept_link.user_id), userId } }
                },
                {
                    nameof(example_link),
                    new Dictionary<string, object> { { nameof(example_link.user_id), userId } }
                },
                {
                    nameof(tone),
                    new Dictionary<string, object> { { nameof(tone.user_id), userId } }
                },
                {
                    nameof(mode),
                    new Dictionary<string, object> { { nameof(mode.user_id), userId } }
                },
                {
                    nameof(register),
                    new Dictionary<string, object> { { nameof(register.user_id), userId } }
                },
                {
                    nameof(nuance),
                    new Dictionary<string, object> { { nameof(nuance.user_id), userId } }
                },
                {
                    nameof(dialect),
                    new Dictionary<string, object> { { nameof(dialect.user_id), userId } }
                }
            };

            var queryRes = await _repository.SelectManyObjects(tables, param) as Dictionary<string, object>;

            if (queryRes == null)
            {
                return null;
            }

            var lstConceptLink = queryRes[nameof(concept_link)] as List<concept_link>;
            //var lstExampleLink = (queryRes[nameof(example_link)] as List<object>).Cast<example_link>().ToList()
            var lstExampleLink = queryRes[nameof(example_link)] as List<example_link>;
            var lstTone = queryRes[nameof(tone)] as List<tone>;
            var lstMode = queryRes[nameof(mode)] as List<mode>;
            var lstRegister = queryRes[nameof(register)] as List<register>;
            var lstNuance = queryRes[nameof(nuance)] as List<nuance>;
            var lstDialect = queryRes[nameof(dialect)] as List<dialect>;

            // Kết quả trả về
            var res = new Dictionary<string, IEnumerable<string>>();
            res.Add(nameof(concept_link),
                lstConceptLink?.Where(x => x.concept_link_type != (int)ConceptLinkType.NoLink)?.Select(x => x.concept_link_name));
            res.Add(nameof(example_link),
                lstExampleLink?.Where(x => x.example_link_type != (int)ExampleLinkType.NoLink)?.Select(x => x.example_link_name));
            res.Add(nameof(tone), lstTone?.Select(x => x.tone_name));
            res.Add(nameof(mode), lstMode?.Select(x => x.mode_name));
            res.Add(nameof(register), lstRegister?.Select(x => x.register_name));
            res.Add(nameof(nuance), lstNuance?.Select(x => x.nuance_name));
            res.Add(nameof(dialect), lstDialect?.Select(x => x.dialect_name));
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

            var lstConcept = exportData[nameof(concept)] as List<concept> ?? new List<concept>();
            var lstConceptRel = exportData[nameof(view_concept_relationship)] as List<view_concept_relationship> ?? new List<view_concept_relationship>();
            var lstExample = exportData[nameof(view_example)] as List<view_example> ?? new List<view_example>();
            var lstExampleRel = exportData[nameof(view_example_relationship)] as List<view_example_relationship> ?? new List<view_example_relationship>();

            // Bind vào sheet concept
            var conceptWs = sheets[TemplateConfig.WorksheetName.Concept];

            conceptWs.Cells[startRow, TemplateConfig.ConceptSheet.Title]
                .LoadFromCollection(lstConcept.Select(x => x.title));

            conceptWs.Cells[startRow, TemplateConfig.ConceptSheet.Description]
                .LoadFromCollection(lstConcept.Select(x => x.description));

            // Bind vào sheet concept relationship
            var conceptRelWs = sheets[TemplateConfig.WorksheetName.ConceptRelationship];

            conceptRelWs.Cells[startRow, TemplateConfig.ConceptRelationshipSheet.ChildConcept]
                .LoadFromCollection(lstConceptRel.Select(x => x.child_name));

            conceptRelWs.Cells[startRow, TemplateConfig.ConceptRelationshipSheet.ParentConcept]
                .LoadFromCollection(lstConceptRel.Select(x => x.parent_name));

            conceptRelWs.Cells[startRow, TemplateConfig.ConceptRelationshipSheet.Relation]
                .LoadFromCollection(lstConceptRel.Select(x => x.concept_link_name));

            // Bind vào sheet example
            var exampleWs = sheets[TemplateConfig.WorksheetName.Example];
            exampleWs.Cells[startRow, TemplateConfig.ExampleSheet.Example]
                .LoadFromCollection(lstExample.Select(x => x.detail_html));

            exampleWs.Cells[startRow, TemplateConfig.ExampleSheet.Tone]
                .LoadFromCollection(lstExample.Select(x => x.tone_name));

            exampleWs.Cells[startRow, TemplateConfig.ExampleSheet.Mode]
                .LoadFromCollection(lstExample.Select(x => x.mode_name));

            exampleWs.Cells[startRow, TemplateConfig.ExampleSheet.Register]
                .LoadFromCollection(lstExample.Select(x => x.register_name));

            exampleWs.Cells[startRow, TemplateConfig.ExampleSheet.Nuance]
                .LoadFromCollection(lstExample.Select(x => x.nuance_name));

            exampleWs.Cells[startRow, TemplateConfig.ExampleSheet.Dialect]
                .LoadFromCollection(lstExample.Select(x => x.dialect_name));

            exampleWs.Cells[startRow, TemplateConfig.ExampleSheet.Note]
               .LoadFromCollection(lstExample.Select(x => x.note));

            // Bind vào sheet example relationship
            var exampleRelWs = sheets[TemplateConfig.WorksheetName.ExampleRelationship];
            exampleRelWs.Cells[startRow, TemplateConfig.ExampleRelationshipSheet.Example]
                .LoadFromCollection(lstExampleRel.Select(x => x.example_html));
            exampleRelWs.Cells[startRow, TemplateConfig.ExampleRelationshipSheet.Concept]
                .LoadFromCollection(lstExampleRel.Select(x => x.concept));
            exampleRelWs.Cells[startRow, TemplateConfig.ExampleRelationshipSheet.Relation]
                .LoadFromCollection(lstExampleRel.Select(x => x.example_link_name));
        }

        #endregion
    }
}
