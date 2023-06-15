using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HUST.Api.Controllers
{
    /// <summary>
    /// Lớp controller cung cấp api helper
    /// </summary>
    public class HelperController : BaseApiController
    {
        #region Fields
        private readonly IExternalApiService _service;
        #endregion

        #region Constructors

        public HelperController(IHustServiceCollection serviceCollection,
            IExternalApiService service) : base(serviceCollection)
        {
            _service = service;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Lấy dữ liệu wordsapi
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        [HttpGet("wordsapi")]
        public async Task<IServiceResult> GetWordsapiResult(string word)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.GetWordsapiResult(word);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Lấy dữ liệu freedictionaryapi
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        [HttpGet("freedictionaryapi")]
        public async Task<IServiceResult> GetFreeDictionaryApiResult(string word)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.GetFreeDictionaryApiResult(word);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Text to speech
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [HttpGet("tts")]
        public async Task<IServiceResult> TextToSpeech(string text)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.TextToSpeech(text);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }

        /// <summary>
        /// Text to speech (stream file)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [HttpGet("tts/stream")]
        public async Task<IActionResult> TextToSpeechStream(string text)
        {
            var res = new ServiceResult();
            try
            {
                var fileBytes = await _service.TextToSpeechStream(text);
                if(fileBytes != null && fileBytes.Length > 0)
                {
                    return File(fileBytes, FileContentType.Audio, "audio");
                }
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, res);
            }
        }

        /// <summary>
        /// Translate
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [HttpGet("translate")]
        public async Task<IServiceResult> Translate(string text)
        {
            var res = new ServiceResult();
            try
            {
                return await _service.Translate(text);
            }
            catch (Exception ex)
            {
                this.ServiceCollection.HandleControllerException(res, ex);
            }

            return res;
        }
        #endregion
    }
}
