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
using System.Threading.Tasks;

namespace HUST.Core.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ cần call external api
    /// </summary>
    public class ExternalApiService : BaseService, IExternalApiService
    {
        #region Field

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
            var key = this.ServiceCollection.ConfigUtil.GetAPIUrl(WordsapiConfigs.Key);
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
                res.OnError(ErrorCode.TooManyRequests, ErrorMessage.TooManyRequests, data: waitTime);
                return res;
            }

            var client = new RestClient(string.Format(url, word));
            var request = new RestRequest();
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
            _accountService.SetThrottleTime(keyThrottle, 10);

            return res.OnSuccess(SerializeUtil.DeserializeObject<dynamic>(response.Content));
        }

        #endregion
    }
}
