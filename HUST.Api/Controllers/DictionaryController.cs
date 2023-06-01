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
    public class DictionaryController : BaseApiController
    {
        #region Fields
        private readonly IDictionaryService _service;
        #endregion

        #region Constructor
        public DictionaryController(IHustServiceCollection serviceCollection, IDictionaryService service) : base(serviceCollection)
        {
            _service = service;
        }
        #endregion

        /// <summary>
        /// Lấy danh sách từ điển đã tạo của người dùng
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_list_dictionary")]
        public async Task<IServiceResult> GetListDictionary()
        {
            var res = new ServiceResult();
            try
            {
                return await _service.GetListDictionary();
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Truy cập vào từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        [HttpGet("load_dictionary")]
        public async Task<IServiceResult> LoadDictionary([FromQuery] string dictionaryId)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.LoadDictionary(dictionaryId);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Thực hiện thêm 1 từ điển mới (có thể kèm việc copy dữ liệu từ 1 từ điển khác đã có)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("add_dictionary")]
        public async Task<IServiceResult> AddDictionary([FromBody] AddDictionaryParam param)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.AddDictionary(param.DictionaryName, param.CloneDictionaryId);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Thực hiện cập nhật tên từ điển
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPatch("update_dictionary")]
        public async Task<IServiceResult> UpdateDictionary([FromBody] Core.Models.DTO.Dictionary param)
        {
            // param.DictionaryId null thì sẽ có exception của .NET => lỗi 400


            var res = new ServiceResult();
            try
            {
                return await _service.UpdateDictionary(param.DictionaryId.ToString(), param.DictionaryName);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Thực hiện xóa từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        [HttpDelete("delete_dictionary")]
        public async Task<IServiceResult> DeleteDictionary([FromQuery] string dictionaryId)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.DeleteDictionary(dictionaryId);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Thực hiện xóa dữ liệu trong từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        [HttpDelete("delete_dictionary_data")]
        public async Task<IServiceResult> DeleteDictionaryData([FromQuery] string dictionaryId)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.DeleteDictionaryData(dictionaryId);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }


        /// <summary>
        /// Thực hiện copy dữ liệu từ từ điển nguồn và gộp vào dữ liệu ở từ điển đích
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("transfer_dictionary")]
        public async Task<IServiceResult> TransferDictionary([FromBody] TransderDictionaryParam param)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.TransferDictionary(param.SourceDictionaryId, param.DestDictionaryId, param.IsDeleteData);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Lấy số lượng concept, example trong 1 từ điển
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_number_record")]
        public async Task<IServiceResult> GetNumberRecord([FromQuery] Guid? dictionaryId)
        {
            var res = new ServiceResult();
            try
            {
                if(dictionaryId == null)
                {
                    dictionaryId = this.ServiceCollection.AuthUtil.GetCurrentDictionaryId();
                }
                return await _service.GetNumberRecord((Guid)dictionaryId);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Lấy template nhập khẩu
        /// </summary>
        /// <returns></returns>
        [HttpGet("download_template_import_dictionary")]
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
                return File(fileBytes, FileContentType.OctetStream, FileDefaultName.DownloadDefaultTemplate);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, res);
            }
        }

        /// <summary>
        /// Lấy template nhập khẩu (test)
        /// </summary>
        /// <returns></returns>
        [HttpGet("download_template_test")]
        public async Task<IActionResult> DowloadTemplateTest()
        {
            IServiceResult res = new ServiceResult();
            try
            {
                var stream = new MemoryStream();
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("SheetTest");
                    package.Save();
                }
                stream.Position = 0;
                string excelName = $"Test.xlsx";
                return File(stream, "application/octet-stream", excelName);

                //stream.Position = 0;
                //string excelName = $"Template-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                //return File(stream, "application/octet-stream", excelName);
                //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return null;
        }

    }
}
