using HUST.Core.Constants;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace HUST.Api.Controllers
{
    public class TemplateController : BaseApiController
    {
        #region Fields
        private readonly ITemplateService _service;
        private readonly IDictionaryService _dictionaryService;

        #endregion

        #region Constructor
        public TemplateController(IHustServiceCollection serviceCollection, 
            ITemplateService service,
            IDictionaryService dictionaryService) : base(serviceCollection)
        {
            _service = service;
            _dictionaryService = dictionaryService;
        }
        #endregion

        /// <summary>
        /// Lấy template nhập khẩu
        /// </summary>
        /// <returns></returns>
        [HttpGet("download")]
        public async Task<IActionResult> DownloadTemplateImportDictionary()
        {
            var res = new ServiceResult();
            try
            {
                var fileBytes = await _service.DowloadTemplateImportDictionary();
                if(fileBytes == null || fileBytes.Length == 0)
                {
                    return StatusCode((int)HttpStatusCode.NoContent);
                }
                return File(fileBytes, FileContentType.OctetStream, TemplateConfig.FileDefaultName.DownloadDefaultTemplate);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, res);
            }
        }

        /// <summary>
        /// Xuất khẩu
        /// </summary>
        /// <returns></returns>
        [HttpGet("export")]
        public async Task<IActionResult> ExportDictionary([FromQuery] string dictionaryId)
        {
            var res = new ServiceResult();
            try
            {
                // Lấy thông tin của từ điển
                var dict = (await _dictionaryService.GetDictionaryById(dictionaryId)).Data as Dictionary;
                if(dict == null)
                {
                    return StatusCode((int)HttpStatusCode.NoContent);
                }

                // Lấy file
                var fileBytes = await _service.ExportDictionary(dict.UserId.ToString(), dict.DictionaryId.ToString());
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    return StatusCode((int)HttpStatusCode.NoContent);
                }

                // Tạo tên file
                var fileName = _service.GetExportFileName(dict.DictionaryName);
                return File(fileBytes, FileContentType.OctetStream, fileName);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, res);
            }
        }

        /// <summary>
        /// Backup data và gửi vào email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        [HttpGet("backup")]
        public async Task<IServiceResult> BackupData([FromQuery] string email, string dictionaryId)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.BackupData(email, dictionaryId);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Nhập khẩu bước 1 (validate) 
        /// </summary>
        /// <returns></returns>
        [HttpPost("import")]
        public async Task<IServiceResult> ImportDictionary()
        {
            var res = new ServiceResult();
            try
            {
                var file = HttpContext.Request.Form.Files[0];
                var dictionaryId = HttpContext.Request.Form["dictionaryId"].ToString();

                //var data =  (await _service.ImportDictionary(dictionaryId, file)).Data as byte[];
                //return File(data, FileContentType.Excel, "File error.xlsx");
                return await _service.ImportDictionary(dictionaryId, file);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }
            return res;
        }

        /// <summary>
        /// Nhập khẩu (lưu dữ liệu)
        /// </summary>
        /// <returns></returns>
        [HttpPost("do_import")]
        public async Task<IServiceResult> DoImportDictionary([FromBody]string importSession)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.DoImport(importSession);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }
            return res;
        }
    }
}
