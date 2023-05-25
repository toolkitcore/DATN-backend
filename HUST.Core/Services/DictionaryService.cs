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

            if (string.IsNullOrEmpty(dictionaryId))
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

        /// <summary>
        /// Thêm 1 từ điển mới 
        /// </summary>
        /// <param name="dictionaryName"></param>
        /// <param name="cloneDictionaryId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> AddDictionary(string dictionaryName, string cloneDictionaryId)
        {
            var res = new ServiceResult();

            // Kiểm tra tham số
            if (string.IsNullOrWhiteSpace(dictionaryName))
            {
                return res.OnError(ErrorCode.Err9000);
            }

            // Bỏ khoảng trắng 2 đầu
            dictionaryName = dictionaryName.Trim();

            var userId = this.ServiceCollection.AuthUtil.GetCurrentUserId();

            // Kiểm tra tên đã được sử dụng chưa
            var existDictionaryWithSameName = await _repository.SelectObject<Models.DTO.Dictionary>(new Dictionary<string, object>
            {
                { nameof(dictionary.user_id), userId },
                { nameof(dictionary.dictionary_name), dictionaryName }
            }) as Models.DTO.Dictionary;

            if (existDictionaryWithSameName != null)
            {
                return res.OnError(ErrorCode.Err2001, ErrorMessage.Err2001);
            }

            // Lấy ra từ điển dùng để clone
            Models.DTO.Dictionary cloneDictionary = null;
            if (!string.IsNullOrEmpty(cloneDictionaryId))
            {
                cloneDictionary = await _repository.SelectObject<Models.DTO.Dictionary>(new Dictionary<string, object>
                {
                    { nameof(dictionary.dictionary_id), cloneDictionaryId },
                    { nameof(dictionary.user_id), userId },
                }) as Models.DTO.Dictionary;
            }


            if (existDictionaryWithSameName != null)
            {
                return res.OnError(ErrorCode.Err2001, ErrorMessage.Err2001);
            }

            // Transaction thêm từ điển mới
            using (var connection = await _repository.CreateConnectionAsync())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    var result = true;
                    var now = DateTime.Now;

                    result = await _repository.Insert(new dictionary
                    {
                        dictionary_id = Guid.NewGuid(),
                        dictionary_name = dictionaryName,
                        user_id = userId,
                        created_date = DateTime.Now,
                        last_view_at = null
                    });

                    if (result && cloneDictionary != null)
                    {
                        // TODO
                    }

                    if (result)
                    {
                        transaction.Commit();
                        res.OnSuccess();
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

        /// <summary>
        /// Thực hiện cập nhật tên từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <param name="dictionaryName"></param>
        /// <returns></returns>
        public async Task<IServiceResult> UpdateDictionary(string dictionaryId, string dictionaryName)
        {
            var res = new ServiceResult();

            // Kiểm tra tham số
            if (string.IsNullOrWhiteSpace(dictionaryName))
            {
                return res.OnError(ErrorCode.Err9000);
            }

            // Bỏ khoảng trắng 2 đầu
            dictionaryName = dictionaryName.Trim();

            var userId = this.ServiceCollection.AuthUtil.GetCurrentUserId();

            // Kiểm tra tên đã được sử dụng chưa
            var existDictionaryWithSameName = await _repository.SelectObject<Models.DTO.Dictionary>(new Dictionary<string, object>
            {
                { nameof(dictionary.user_id), userId },
                { nameof(dictionary.dictionary_name), dictionaryName }
            }) as Models.DTO.Dictionary;

            if (existDictionaryWithSameName != null)
            {
                if (string.Equals(existDictionaryWithSameName.DictionaryId.ToString(), dictionaryId))
                {
                    return res.OnSuccess();
                }
                return res.OnError(ErrorCode.Err2001, ErrorMessage.Err2001);
            }

            // Kiểm tra người dùng có dictionary này không
            // Vì có thể dictionaryId tồn tại, nhưng không phải của user này
            var dict = await _repository.SelectObject<Models.DTO.Dictionary>(new Dictionary<string, object>
            {
                { nameof(dictionary.user_id), userId },
                { nameof(dictionary.dictionary_id), dictionaryId }
            }) as Models.DTO.Dictionary;
            if (dict == null)
            {
                return res.OnError(ErrorCode.Err2000, ErrorMessage.Err2000);
            }

            // Cập nhật tên từ điển
            var result = await _repository.Update(new
            {
                dictionary_id = dictionaryId,
                dictionary_name = dictionaryName,
                modified_date = DateTime.Now
            });

            if (result)
            {
                return res.OnSuccess();
            }
            else
            {
                return res.OnError(ErrorCode.Err9999);
            }
        }
        #endregion

    }


}
