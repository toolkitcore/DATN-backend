using HUST.Core.Models.DTO;
using HUST.Core.Models.ServerObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace HUST.Core.Interfaces.Service
{
    /// <summary>
    /// Service xử lý nghiệp vụ cần call external api
    /// </summary>
    public interface IExternalApiService
    {
        /// <summary>
        /// Lấy kết quả request wordsapi
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        Task<IServiceResult> GetWordsapiResult(string word);

        /// <summary>
        /// Lấy kết quả request free dictionaryapi
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        Task<IServiceResult> GetFreeDictionaryApiResult(string word);

        /// <summary>
        /// TextToSpeech
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        Task<IServiceResult> TextToSpeech(string text, string lang = "en");

        /// <summary>
        /// TextToSpeech (Stream)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        Task<byte[]> TextToSpeechStream(string text, string lang = "en");

        /// <summary>
        /// Translate
        /// </summary>
        /// <param name="text"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        Task<IServiceResult> Translate(string text, string from = "en", string to = "vi");
    }
}
