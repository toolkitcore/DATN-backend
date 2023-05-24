using HUST.Core.Constants;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.Entity;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using User = HUST.Core.Models.DTO.User;

namespace HUST.Core.Services
{
    /// <summary>
    /// Serivce xử lý dictionary
    /// </summary>
    public class DictionaryService : BaseService, IDictionaryService
    {
        #region Field

        private readonly IDictionaryRepository _repository;
        private readonly IAccountService _accountService;

        #endregion

        #region Constructor

        public DictionaryService(IDictionaryRepository dictionaryRepository,
            IAccountService accountService,
            IHustServiceCollection serviceCollection) : base(serviceCollection)
        {
            _repository = dictionaryRepository;
            _accountService = accountService;
        }
        #endregion

        #region Method
        /// <summary>
        /// Lấy danh sách từ điển đã tạo của người dùng
        /// </summary>
        /// <returns></returns>
        public async Task<IServiceResult> GetListDictionary()
        {
            var res = new ServiceResult();

            var userId = this.ServiceCollection.AuthUtil.GetCurrentUserId();
            var lstDictionary = await _repository.SelectObjects<Models.DTO.Dictionary>(new Dictionary<string, object>()
            {
                { nameof(Models.Entity.dictionary.user_id), userId }
            });

            // Mặc định sort theo thời điểm truy cập > thời điểm tạo > tên
            // Client có thể hỗ trợ sort ưu tiên theo tên
            // Nhưng nên để <dictionary đang load> luôn ở vị trí đầu tiên
            res.Data = lstDictionary
                .OrderByDescending(x => x.LastViewAt)
                .ThenByDescending(x => x.CreatedDate)
                .ThenBy(x => x.DictionaryName)
                .ToList();

            return res;
        }

        /// <summary>
        /// Truy cập vào từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> LoadDictionary(string dictionaryId)
        {
            var res = new ServiceResult();
            var user = this.ServiceCollection.AuthUtil.GetCurrentUser();
            var oldDictionaryId = user.DictionaryId;

            if(string.IsNullOrEmpty(dictionaryId))
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }

            // Nếu load chính dictionary hiện tại thì không xử lý gì cả
            if (dictionaryId == oldDictionaryId?.ToString())
            {
                return res;
            }

            var dict = await _repository.SelectObject<Models.DTO.Dictionary>(new Dictionary<string, object>
            {
                { nameof(dictionary.dictionary_id), dictionaryId },
                { nameof(dictionary.user_id), user.UserId }
            }) as Models.DTO.Dictionary;

            if (dict == null)
            {
                return res.OnError(ErrorCode.Err2000, ErrorMessage.Err2000);
            }

            // Cập nhật session
            user.DictionaryId = dict.DictionaryId;
            _accountService.RemoveCurrentSession(); // Xóa token, session nếu có
            var sessionId = _accountService.GenerateSession(user); // Sinh token, session
            _accountService.SetResponseSessionCookie(sessionId); // Gán session vào response trả về

            // Cập nhật thời điểm truy cập cho dictionary
            using (var connection = await _repository.CreateConnectionAsync())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    var result = true;
                    var now = DateTime.Now;

                    result = await _repository.Update(new
                    {
                        dictionary_id = dict.DictionaryId,
                        last_view_at = now
                    });

                    if (result)
                    {
                        result = await _repository.Update(new
                        {
                            dictionary_id = oldDictionaryId,
                            last_view_at = now.AddMinutes(-1) // 1 phút trước
                        });
                    }

                    if (result)
                    {
                        transaction.Commit();
                        res.OnSuccess(new
                        {
                            SessionId = sessionId,
                            user.UserId,
                            user.UserName,
                            dict.DictionaryId,
                            dict.DictionaryName,
                            LastViewAt = now
                        });
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

        public Task<IServiceResult> AddDictionary()
        {
            throw new NotImplementedException();
        }
        #endregion

    }


}
