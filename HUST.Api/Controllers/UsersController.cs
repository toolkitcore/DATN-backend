//using HUST.Core.Interfaces.Service;
//using HUST.Core.Models.DTO;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Threading.Tasks;

//namespace HUST.Api.Controllers
//{

//    public class UsersController : BaseEntitiesController<User>
//    {
//        private readonly IWebHostEnvironment _hostEnvironment;
//        IUserService _userService;

//        #region Contructor
//        public UsersController(IWebHostEnvironment hostEnvironment, IUserService userService) : base(userService)
//        {
//            this._hostEnvironment = hostEnvironment;
//            this._userService = userService;
//        }

//        #endregion

//        // POST api/<BaseEntitiesController>
//        [HttpGet("GetUserId")]
//        public async Task<IActionResult> GetUserIdByToken(string token)
//        {
//            try
//            {
//                var serviceResult = await _userService.GetUserIdByToken(token);
//                if(serviceResult.Data != null)
//                {

//                    return Ok(serviceResult);
//                }
//                return NoContent();
//            }
//            catch (Exception e)
//            {

//                return StatusCode(500, new ServiceResult(e));
//            }
//        }
//    }
//}
