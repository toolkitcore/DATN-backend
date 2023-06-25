using HUST.Core.Constants;
using HUST.Core.Enums;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Interfaces.Service;
using HUST.Core.Models.DTO;
using HUST.Core.Models.Entity;
using HUST.Core.Models.Param;
using HUST.Core.Models.ServerObject;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        private readonly StorageUtil _storage;
        #endregion

        #region Constructor

        public DictionaryService(IDictionaryRepository dictionaryRepository,
            IAccountService accountService,
            StorageUtil storage,
            IHustServiceCollection serviceCollection) : base(serviceCollection)
        {
            _repository = dictionaryRepository;
            _accountService = accountService;
            _storage = storage;
        }
        #endregion

        #region Method
        /// <summary>
        /// Lấy thông tin từ điển theo id
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> GetDictionaryById(string dictionaryId)
        {
            var res = new ServiceResult();

            if(string.IsNullOrEmpty(dictionaryId))
            {
                dictionaryId = this.ServiceCollection.AuthUtil.GetCurrentDictionaryId()?.ToString();
            }

            res.Data = await _repository.SelectObject<Models.DTO.Dictionary>(new Dictionary<string, object>()
            {
                { nameof(Models.Entity.dictionary.dictionary_id), dictionaryId }
            }) as Models.DTO.Dictionary;

            return res;
        }

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
                    }, transaction);

                    if (result)
                    {
                        result = await _repository.Update(new
                        {
                            dictionary_id = oldDictionaryId,
                            last_view_at = now.AddMinutes(-1) // 1 phút trước
                        }, transaction);
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

            // Transaction thêm từ điển mới: thêm từ điển + clone dữ liệu nếu cần
            // Exception sẽ không được bắt tại đây (mà bắt ở controller) => không tự động rollback được transaction
            // => có thể bổ sung try catch để bắt exception rồi rollback
            // Có thể không dùng transaction: vì insert luôn trả về true, store luôn trả về true
            var newDictionaryId = Guid.NewGuid();
            _ = await _repository.Insert(new dictionary
            {
                dictionary_id = newDictionaryId,
                dictionary_name = dictionaryName,
                user_id = userId,
                created_date = DateTime.Now,
                last_view_at = null
            });

            if (cloneDictionary != null)
            {
                _ = await _repository.CloneDictionaryData(cloneDictionary.DictionaryId, newDictionaryId);
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

        /// <summary>
        /// Thực hiện xóa từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> DeleteDictionary(string dictionaryId)
        {
            var res = new ServiceResult();
            var currentUserId = this.ServiceCollection.AuthUtil.GetCurrentUserId();
            var currentDictionaryId = this.ServiceCollection.AuthUtil.GetCurrentDictionaryId();

            var lstDictionary = (await _repository.SelectObjects<Models.DTO.Dictionary>(new Dictionary<string, object>()
            {
                { nameof(dictionary.user_id), currentUserId}
            }))?.ToList();

            // Nếu không có bất cứ từ điển nào
            if (lstDictionary == null || lstDictionary.Count == 0)
            {
                return res.OnError(ErrorCode.Err9999);
            }

            // Kiểm tra người dùng có sở hữu từ điển này không
            var dictionaryItem = lstDictionary.Find(x => string.Equals(dictionaryId, x.DictionaryId.ToString()));
            if (dictionaryItem == null)
            {
                return res.OnError(ErrorCode.Err2000, ErrorMessage.Err2000);
            }

            // Kiểm tra từ điển có đang sử dụng hay không
            var isDictionaryInUse = string.Equals(dictionaryId, currentDictionaryId?.ToString())
                || (lstDictionary.Count == 1 && string.Equals(dictionaryId, lstDictionary[0].DictionaryId.ToString())); // chú ý ToString
            if (isDictionaryInUse)
            {
                return res.OnError(ErrorCode.Err2002, ErrorMessage.Err2002);
            }


            var result = await _repository.Delete(new
            {
                dictionary_id = dictionaryItem.DictionaryId,
                user_id = currentUserId
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

        /// <summary>
        /// Thực hiện xóa dữ liệu từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> DeleteDictionaryData(string dictionaryId)
        {
            var res = new ServiceResult();
            var currentUserId = this.ServiceCollection.AuthUtil.GetCurrentUserId();

            // Kiểm tra từ điển
            var dict = await _repository.SelectObject<Models.DTO.Dictionary>(new Dictionary<string, object>
            {
                { nameof(dictionary.user_id), currentUserId },
                { nameof(dictionary.dictionary_id), dictionaryId }
            }) as Models.DTO.Dictionary;

            // Nếu không có từ điển
            if (dict == null)
            {
                return res.OnError(ErrorCode.Err2000, ErrorMessage.Err2000);
            }

            _ = await _repository.DeleteDictionaryData(dict.DictionaryId);

            return res.OnSuccess();
        }

        /// <summary>
        /// Thực hiện copy dữ liệu từ từ điển nguồn và gộp vào dữ liệu ở từ điển đích
        /// </summary>
        /// <param name="sourceDictionaryId"></param>
        /// <param name="destDictionaryId"></param>
        /// <param name="isDeleteData"></param>
        /// <returns></returns>
        public async Task<IServiceResult> TransferDictionary(string sourceDictionaryId, string destDictionaryId, bool? isDeleteData)
        {
            var res = new ServiceResult();

            if (string.IsNullOrEmpty(sourceDictionaryId) || string.IsNullOrEmpty(destDictionaryId))
            {
                return res.OnError(ErrorCode.Err9999);
            }

            // Kiểm tra số lượng bản ghi trong từ điển nguồn
            var srcRecords = await _repository.GetNumberRecord(Guid.Parse(sourceDictionaryId));
            if((srcRecords.NumberConcept ?? 0) == 0 && (srcRecords.NumberExample ?? 0) == 0)
            {
                return res.OnError(ErrorCode.Err2003, ErrorMessage.Err2003);
            }

            _ = await _repository.TransferDictionaryData(
                Guid.Parse(sourceDictionaryId),
                Guid.Parse(destDictionaryId),
                isDeleteData ?? false);

            return res;
        }

        /// <summary>
        /// Lấy số lượng concept, example trong 1 từ điển
        /// </summary>
        /// <param name="dictionaryId"></param>
        /// <returns></returns>
        public async Task<IServiceResult> GetNumberRecord(Guid dictionaryId)
        {
            var res = new ServiceResult();

            var data = await _repository.GetNumberRecord(dictionaryId);

            res.Data = data;

            return res;
        }
        #endregion
    }


}
