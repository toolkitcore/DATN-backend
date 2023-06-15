using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HUST.Core.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ cần call external api
    /// </summary>
    public class ExternalApiService : BaseService, IExternalApiService
    {
        #region Field
        private const int ThrottleTime = 6; // seconds
        private const int ThrottleTimeImportant = 10; // seconds
        private readonly ICacheExternalWordApiRepository _cacheRepository;
        private readonly ICacheSqlUtil _cacheSql;
        private readonly IAccountService _accountService;

        #endregion

        #region Constructor

        public ExternalApiService(
            ICacheExternalWordApiRepository cacheRepository,
            ICacheSqlUtil cacheSql,
            IAccountService accountService,
            IHustServiceCollection serviceCollection) : base(serviceCollection)
        {
            _cacheRepository = cacheRepository;
            _cacheSql = cacheSql;
            _accountService = accountService;
        }



        #endregion

        #region Method

        /// <summary>
        /// Lấy kết quả request wordsapi
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public async Task<IServiceResult> GetWordsapiResult(string word)
        {
            var res = new ServiceResult();

            // Kiểm tra đầu vào
            if (string.IsNullOrWhiteSpace(word))
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }
            word = word.ToLower().Trim();

            // Lấy ra config trong appsetting
            var url = this.ServiceCollection.ConfigUtil.GetAPIUrl(WordsapiConfigs.Url);
            var apiKey = this.ServiceCollection.ConfigUtil.GetAPIUrl(WordsapiConfigs.Key);
            var strMaxRequestPerDay = this.ServiceCollection.ConfigUtil.GetAPIUrl(WordsapiConfigs.MaxRequestPerDay);
            var parseRes = int.TryParse(strMaxRequestPerDay, out var maxRequestPerDay);
            if (string.IsNullOrEmpty(url) || !parseRes)
            {
                return res.OnError(ErrorCode.Err9999);
            }
            maxRequestPerDay = maxRequestPerDay > 0 ? maxRequestPerDay : 100;

            // Kiểm tra có data lưu sẵn không
            var existCacheData = await _cacheRepository.SelectObject<CacheExternalWordApi>(new
            {
                word = word,
                external_api_type = (int)ExternalApiType.WordsApi
            }) as CacheExternalWordApi;

            if (existCacheData != null)
            {
                return res.OnSuccess(SerializeUtil.DeserializeObject<dynamic>(existCacheData.Value));
            }

            // Trường hợp không có dữ liệu lưu sẵn thì phải call api
            // Kiểm tra số lần request
            var cacheKeyNumberRequest = CacheKey.WordsapiNumberRequestPerDayCache;
            var strNumberRequest = await _cacheSql.GetCache(cacheKeyNumberRequest);
            if (!string.IsNullOrEmpty(strNumberRequest) && int.Parse(strNumberRequest) >= maxRequestPerDay)
            {
                return res.OnError(ErrorCode.TooManyRequests, ErrorMessage.TooManyRequests);
            }

            // Kiểm tra thời gian chặn api call liên tục
            var keyThrottle = $"GetWordsapiResult";
            var waitTime = _accountService.GetThrottleTime(keyThrottle);
            if (waitTime > 0)
            {
                //res.OnError(ErrorCode.TooManyRequests, ErrorMessage.TooManyRequests, data: waitTime);
                //return res;
                await Task.Delay((int)(waitTime * 1000));
            }

            var client = new RestClient(string.Format(url, word));
            var request = new RestRequest();
            if (!string.IsNullOrEmpty(apiKey))
            {
                request.AddHeader("X-Mashape-Key", apiKey);
            }
            var response = await client.GetAsync(request);

            // Cache lại dữ liệu lấy được thành công
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var cacheData = new cache_external_word_api
                {
                    external_api_type = (int)ExternalApiType.WordsApi,
                    word = word,
                    route = word,
                    value = response.Content,
                    created_date = DateTime.Now
                };
                await _cacheRepository.Insert(cacheData);
            }

            // Tăng biến đếm số lần request
            if (strNumberRequest == null)
            {
                await _cacheSql.SetCache(cacheKeyNumberRequest, "1", null, TimeSpan.FromDays(1), isSystem: true);
            }
            else
            {
                var currNumberRequest = int.Parse(strNumberRequest) + 1;
                await _cacheSql.UpdateOnlyCacheValue(cacheKeyNumberRequest, currNumberRequest.ToString());
            }

            // Với trường hợp phải call external api => set thời gian chặn call api liên tục
            _accountService.SetThrottleTime(keyThrottle, ThrottleTime);

            return res.OnSuccess(SerializeUtil.DeserializeObject<dynamic>(response.Content));
        }

        /// <summary>
        /// Lấy kết quả request free dictionaryapi
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public async Task<IServiceResult> GetFreeDictionaryApiResult(string word)
        {
            var res = new ServiceResult();

            // Kiểm tra đầu vào
            if (string.IsNullOrWhiteSpace(word))
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }
            word = word.ToLower().Trim();

            // Lấy ra config trong appsetting
            var url = this.ServiceCollection.ConfigUtil.GetAPIUrl(FreedictionaryapiConfigs.Url);
            if (string.IsNullOrEmpty(url))
            {
                return res.OnError(ErrorCode.Err9999);
            }

            // Kiểm tra có data lưu sẵn không
            var existCacheData = await _cacheRepository.SelectObject<CacheExternalWordApi>(new
            {
                word = word,
                external_api_type = (int)ExternalApiType.FreeDictionaryApi
            }) as CacheExternalWordApi;

            if (existCacheData != null)
            {
                return res.OnSuccess(SerializeUtil.DeserializeObject<dynamic>(existCacheData.Value));
            }

            // Kiểm tra thời gian chặn api call liên tục
            var keyThrottle = $"GetFreeDictionaryApiResult";
            var waitTime = _accountService.GetThrottleTime(keyThrottle);
            if (waitTime > 0)
            {
                //res.OnError(ErrorCode.TooManyRequests, ErrorMessage.TooManyRequests, data: waitTime);
                //return res;
                await Task.Delay((int)(waitTime * 1000));
            }


            var client = new RestClient(url);
            var request = new RestRequest(word);
            var response = await client.GetAsync(request);

            var content = response.Content;
            // Cache lại dữ liệu lấy được thành công
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string pattern = "((https://api.dictionaryapi.dev/media/pronunciations)((?!.mp3).)*(.mp3))";
                var lstMp3 = new List<string>();
                foreach (Match match in Regex.Matches(content, pattern))
                {
                    if (match.Success && match.Groups.Count > 0)
                    {
                        lstMp3.Add(match.Groups[1].Value);
                    }
                }

                var savedData = new
                {
                    pronunciation = new
                    {
                        us = lstMp3.Find(x => Regex.IsMatch(x, "-us.mp3")),
                        uk = lstMp3.Find(x => Regex.IsMatch(x, "-uk.mp3")),
                        other = lstMp3.Find(x => !Regex.IsMatch(x, "-us.mp3") && !Regex.IsMatch(x, "-uk.mp3"))
                    },
                    content = SerializeUtil.DeserializeObject<dynamic>(content)
                };
                content = SerializeUtil.SerializeObject(savedData);
                var cacheData = new cache_external_word_api
                {
                    external_api_type = (int)ExternalApiType.FreeDictionaryApi,
                    word = word,
                    route = word,
                    value = content,
                    created_date = DateTime.Now
                };
                await _cacheRepository.Insert(cacheData);
            }

            // Với trường hợp phải call external api => set thời gian chặn call api liên tục
            _accountService.SetThrottleTime(keyThrottle, ThrottleTime);

            return res.OnSuccess(SerializeUtil.DeserializeObject<dynamic>(content));
        }

        /// <summary>
        /// Text to speech
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<IServiceResult> TextToSpeech(string text)
        {
            var res = new ServiceResult();

            // Kiểm tra đầu vào
            if (string.IsNullOrWhiteSpace(text))
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }
            text = text.ToLower().Trim();

            // Lấy ra config trong appsetting
            var url = this.ServiceCollection.ConfigUtil.GetAPIUrl(HelperAppConfigs.TextToSpeechUrl);
            var key = this.ServiceCollection.ConfigUtil.GetAPIUrl(HelperAppConfigs.Key);
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(key))
            {
                return res.OnError(ErrorCode.Err9999);
            }

            // Kiểm tra thời gian chặn api call liên tục
            var keyThrottle = $"TextToSpeech";
            var waitTime = _accountService.GetThrottleTime(keyThrottle);
            if (waitTime > 0)
            {
                await Task.Delay((int)(waitTime * 1000));
            }

            // Call api
            var client = new RestClient(string.Format(url, text));
            var request = new RestRequest();
            request.AddHeader(AuthKey.HelperAppApiKey, key); // Add key vào header
            var response = await client.GetAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                res.Data = new
                {
                    ContentType = FileContentType.Audio,
                    Base64Data = Convert.ToBase64String(response.RawBytes)
                };
            }

            // Set thời gian chặn call api liên tục
            _accountService.SetThrottleTime(keyThrottle, ThrottleTime);

            return res;
        }

        /// <summary>
        /// Lấy kết quả request free dictionaryapi
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<byte[]> TextToSpeechStream(string text)
        {
            // Kiểm tra đầu vào
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            text = text.ToLower().Trim();

            // Lấy ra config trong appsetting
            var url = this.ServiceCollection.ConfigUtil.GetAPIUrl(HelperAppConfigs.TextToSpeechUrl);
            var key = this.ServiceCollection.ConfigUtil.GetAPIUrl(HelperAppConfigs.Key);
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(key))
            {
                return null;
            }

            // Kiểm tra thời gian chặn api call liên tục
            var keyThrottle = $"TextToSpeechStream";
            var waitTime = _accountService.GetThrottleTime(keyThrottle);
            if (waitTime > 0)
            {
                await Task.Delay((int)(waitTime * 1000));
            }

            // Call api
            var client = new RestClient(string.Format(url, text));
            var request = new RestRequest();
            request.AddHeader(AuthKey.HelperAppApiKey, key); // Add key vào header
            var response = await client.GetAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.RawBytes;
            }

            // Set thời gian chặn call api liên tục
            _accountService.SetThrottleTime(keyThrottle, ThrottleTime);

            return null;
        }

        /// <summary>
        /// Translate
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<IServiceResult> Translate(string text)
        {
            var res = new ServiceResult();

            // Kiểm tra đầu vào
            if (string.IsNullOrWhiteSpace(text))
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }
            text = text.ToLower().Trim();

            // Lấy ra config trong appsetting
            var url = this.ServiceCollection.ConfigUtil.GetAPIUrl(HelperAppConfigs.TranslateTestUrl);
            var key = this.ServiceCollection.ConfigUtil.GetAPIUrl(HelperAppConfigs.Key);
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(key))
            {
                return res.OnError(ErrorCode.Err9999);
            }

            // Kiểm tra thời gian chặn api call liên tục
            var keyThrottle = $"Translate";
            var waitTime = _accountService.GetThrottleTime(keyThrottle);
            if (waitTime > 0)
            {
                await Task.Delay((int)(waitTime * 1000));
            }

            // Call api
            var client = new RestClient(string.Format(url, text));
            var request = new RestRequest();
            request.AddHeader(AuthKey.HelperAppApiKey, key); // Add key vào header

            var response = await client.GetAsync(request);

            // Set thời gian chặn call api liên tục
            _accountService.SetThrottleTime(keyThrottle, ThrottleTimeImportant);

            return res.OnSuccess(new
            {
                StatusCode = response.StatusCode,
                StatusDescription = response.StatusDescription,
                Content = response.Content
            });
        }
        #endregion
    }
}
