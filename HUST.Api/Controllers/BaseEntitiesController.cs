//using HUST.Core.Enums;
//using HUST.Core.Interfaces.Service;
//using HUST.Core.Models.ServerObject;
//using HUST.Core.Utils;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace HUST.Api.Controllers
//{
//    /// <summary>
//    /// Lớp controller cơ sở
//    /// </summary>
//    /// <typeparam name="TEntity">Lớp thực thể</typeparam>
//    [Route("api/v1/[controller]")]
//    [ApiController]
//    public class BaseEntitiesController<TEntity> : ControllerBase where TEntity : class
//    {
//        #region Fields
//        public readonly IHustServiceCollection ServiceCollection;
//        protected readonly IBaseService<TEntity> _baseService;

//        #endregion

//        #region Constructors

//        public BaseEntitiesController(IBaseService<TEntity> baseService, IHustServiceCollection serviceCollection)
//        {
//            _baseService = baseService;
//            ServiceCollection = serviceCollection;
//        }

//        #endregion

//        #region Methods

//        /// <summary>
//        /// API lấy ra danh sách tất cả bản ghi
//        /// </summary>
//        /// <returns>
//        /// - 200: lấy thành công, hiển thị danh sách bản ghi
//        /// - 204: không có dữ liệu
//        /// - 500: xảy ra exception
//        /// </returns>
//        [HttpGet]
//        public async Task<IServiceResult> GetAllEntites()
//        {
//            var res = new ServiceResult();
//            try
//            {
//                // Lấy dữ liệu trả về thông qua service
//                var entities = (await _baseService.GetAll()).Data;
//                res.OnSuccess(entities);
//            }
//            catch (Exception ex)
//            {
//                this.ServiceCollection.HandleControllerException(res, ex);
//            }
//            return res;
//        }

//        /// <summary>
//        /// API lấy ra đối tượng thực thể theo khóa chính(id)
//        /// </summary>
//        /// <param name="entityId">Khóa chính(id)</param>
//        /// <returns>
//        /// - 200: lấy thành công
//        /// - 204: không có dữ liệu
//        /// - 400: lỗi entityId không đúng kiểu guid
//        /// - 500: xảy ra exception
//        /// </returns>
//        [HttpGet("{entityId}")]
//        public async Task<IServiceResult> GetEntityById(string entityId)
//        {
//            var res = new ServiceResult();
//            try
//            {
//                // parse string sang kiểu Guid
//                if (Guid.TryParse(entityId, out var entityIdGuid))
//                {
//                    // Lấy dữ liệu trả về thông qua service
//                    var serviceResult = await _baseService.GetById(entityIdGuid);
//                    var entity = serviceResult.Data;
//                    res.OnSuccess(entity);
//                } else
//                {
//                    res.OnError(null, "BadRequest");
//                }
//            }
//            catch (Exception ex)
//            {
//                this.ServiceCollection.HandleControllerException(res, ex);
//            }
//            return res;
//        }

//        /// <summary>
//        /// API thêm mới đối tượng thực thể
//        /// </summary>
//        /// <param name="entity">Đối tượng cần thêm mới</param>
//        /// <returns>
//        /// - 201: thêm thành công
//        /// - 204: không có bản ghi nào được thêm
//        /// - 200: thất bại do có lỗi khi xử lý nghiệp vụ
//        /// - 500: xảy ra exception
//        /// </returns>
//        /// CreatedBy: PTHIEU (17/8/2021)
//        [HttpPost]
//        public async Task<ServiceResult> PostEntity(TEntity entity)
//        {
//            var res = new ServiceResult();
//            try
//            {
//                // Thực hiện service thêm mới
//                var serviceResult = await _baseService.Insert(entity);

//                // TH thêm mới thành công: IsSuccess == true, số bản ghi thêm mới > 0
//                if (serviceResult.Status == ServiceResultStatus.Success && (bool)serviceResult.Data)
//                {
//                    //return StatusCode(201, serviceResult);
//                }

//                // Trường hợp thêm mới thất bại do xử lý nghiệp vụ
//                // Trường hợp số bản ghi thêm được bằng 0
//                //return Ok(serviceResult);
//            }
//            catch (Exception ex)
//            {
//                this.ServiceCollection.HandleControllerException(res, ex);
//            }
//            return res;
//        }

//        /// <summary>
//        /// API chỉnh sửa thông tin đối tượng thực thể
//        /// </summary>
//        /// <param name="entity">Đối tượng cần cập nhật</param>
//        /// <returns>
//        /// - 200: chỉnh sửa thành công/ hoặc xảy ra lỗi nghiệp vụ
//        /// - 400: lỗi request, entityId không đúng kiểu guid
//        /// - 500: xảy ra exception
//        /// </returns>
//        /// CreatedBy: PTHIEU (17/8/2021)
//        [HttpPut("{entityId}")]
//        public async Task<IActionResult> PutEntity(string entityId, TEntity entity)
//        {
//            try
//            {
//                // parse string sang kiểu Guid
//                if (Guid.TryParse(entityId, out var entityIdGuid))
//                {
//                    // Thực hiện service cập nhật/chỉnh sửa
//                    // Trường hợp thực hiện thất bại do lỗi nghiệp vụ (serviceResult.IsSuccess == false)
//                    // Trường hợp affected rows = 0
//                    // Đều trả về mã 200
//                    return Ok(await _baseService.Update(entityIdGuid, entity));
//                }

//                // trả về mã lỗi 400 nếu không parse được entityId sang Guid
//                return BadRequest(new ServiceResult
//                {
//                    IsSuccess = false,
//                    UserMsg = Core.Properties.Resources.Error,
//                    DevMsg = Core.Properties.Resources.ErrorRequest,
//                    ErrorCode = ErrorCode.ErrorCodeBadRequest
//                });

//            }
//            catch (Exception e)
//            {
//                return StatusCode(500, new ServiceResult(e));
//            }
//        }


//        /// <summary>
//        /// API xóa đối tượng thực thể theo khóa chính(id)
//        /// </summary>
//        /// <param name="entityId">Khóa chính(id)</param>
//        /// <returns>
//        /// - 200: xóa thành công
//        /// - 400: lỗi request, entityId không đúng kiểu Guid
//        /// - 500: xảy ra exception
//        /// </returns>
//        /// CreatedBy: PTHIEU (17/8/2021)
//        [HttpDelete("{entityId}")]
//        public async Task<IActionResult> DeleteEntity(string entityId)
//        {
//            try
//            {
//                // parse string sang kiểu guid
//                if (Guid.TryParse(entityId, out var entityIdGuid))
//                {
//                    // Thực hiện service xóa bản ghi
//                    // TH affected row = 0
//                    // vẫn trả về mã 200 (có thể trả về mã 404)
//                    return Ok(await _baseService.Delete(entityIdGuid));
//                }

//                // trả về mã lỗi 400 nếu không parse được entityId sang Guid
//                return BadRequest(new ServiceResult
//                {
//                    IsSuccess = false,
//                    UserMsg = Core.Properties.Resources.Error,
//                    DevMsg = Core.Properties.Resources.ErrorRequest,
//                    ErrorCode = ErrorCode.ErrorCodeBadRequest
//                });

//            }
//            catch (Exception e)
//            {
//                return StatusCode(500, new ServiceResult(e));
//            }
//        }

//        #endregion
//    }
//}
