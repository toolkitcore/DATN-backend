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
using System.Threading.Tasks;
using User = HUST.Core.Models.DTO.User;

namespace HUST.Core.Services
{
    /// <summary>
    /// Serivce xử lý user
    /// </summary>
    public class UserService : BaseService, IUserService
    {
        #region Field

        private readonly IUserRepository _repository;
        private readonly StorageUtil _storage;
        #endregion

        #region Constructor

        public UserService(IUserRepository userRepository,
            IHustServiceCollection serviceCollection,
            StorageUtil storage) : base(serviceCollection)
        {
            _repository = userRepository;
            _storage = storage;
        }

        #endregion

        #region Method

        /// <summary>
        /// Thay đổi mật khẩu
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        /// Modified by PTHIEU 20.06.2023: 
        /// - Sửa lỗi check password cũ luôn fail do hash không bằng nhau (sai logic check)
        /// - Sửa lỗi lưu password mới mà chưa hash
        public async Task<IServiceResult> UpdatePassword(string oldPassword, string newPassword)
        {
            var res = new ServiceResult();

            // Kiểm tra thông tin đăng nhập
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            {
                return res.OnError(ErrorCode.Err9000, ErrorMessage.Err9000);
            }

            var userId = this.ServiceCollection.AuthUtil.GetCurrentUserId();
            var user = await _repository.SelectObject<User>(new Dictionary<string, object>()
            {
                { nameof(Models.Entity.user.user_id), userId },
                //{ nameof(Models.Entity.user.password), SecurityUtil.HashPassword(oldPassword) }
            }) as User;

            if(user == null || !SecurityUtil.VerifyPassword(oldPassword, user.Password))
            {
                return res.OnError(ErrorCode.Err1000, ErrorMessage.Err1000);
            }

            await _repository.Update(new
            {
                user_id = userId,
                password = SecurityUtil.HashPassword(newPassword)
            });

            return res.OnSuccess();
        }

        /// <summary>
        /// Lấy các thông tin cá nhân của người dùng
        /// </summary>
        /// <returns></returns>
        public async Task<IServiceResult> GetUserInfo()
        {
            var res = new ServiceResult();

            var userId = this.ServiceCollection.AuthUtil.GetCurrentUserId();
            var user = await _repository.SelectObject<User>(new Dictionary<string, object>()
            {
                { nameof(Models.Entity.user.user_id), userId }
            }) as User;

            user.Password = null;

            return res.OnSuccess(user); ;
        }

        /// <summary>
        /// Cập nhật các thông tin cá nhân của người dùng
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<IServiceResult> UpdateUserInfo(UpdateUserInfoParam param)
        {
            var res = new ServiceResult();

            // Cập nhật thông tin user
            var userId = this.ServiceCollection.AuthUtil.GetCurrentUserId();
            var user = await _repository.SelectObject<User>(new Dictionary<string, object>()
            {
                { nameof(Models.Entity.user.user_id), userId }
            }) as User;

            if(user == null)
            {
                return res.OnError(ErrorCode.Err9999);
            }

            // TODO: Xem xét giá trị Birthday có đúng không, có bị lệch ngày giờ không

            // Upload ảnh đại diện
            string avatarLink = null;
            if (param.Avatar != null)
            {
                if (!FunctionUtil.IsImageFile(param.Avatar))
                {
                    return res.OnError(ErrorCode.Err9003, ErrorMessage.Err9003);
                }

                if (!FunctionUtil.IsValidFileSize(param.Avatar))
                {
                    return res.OnError(ErrorCode.Err9002, ErrorMessage.Err9002);
                }

                if (param.Avatar.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        param.Avatar.CopyTo(ms);
                        var fileBytes = ms.ToArray();

                        // Để bảo mật hơn thì dùng Guid.NewGuid().ToString()
                        // Để tiện hơn thì dùng luôn user id (đè lại, không cần xóa ảnh cũ)
                        avatarLink = await _storage.UploadAsync(StoragePath.Avatar, userId.ToString(), fileBytes);
                    }

                    // Xóa ảnh cũ
                    //_ = await _storage.DeleteAsync(StoragePath.Avatar, fileName);
                }
            }

            var result = await _repository.Update(new 
            {
                user_id = (Guid)userId,
                display_name = param.DisplayName?.Trim(),
                full_name = param.FullName?.Trim(),
                birthday = param.Birthday != null ? DateTime.Parse(param.Birthday) : (DateTime?)null,
                position = param.Position?.Trim(),
                avatar = avatarLink ?? user.Avatar
            });

            if(!result)
            {
                return res.OnError(ErrorCode.Err9999);
            }

            res.OnSuccess();
            res.Data = new
            {
                DisplayName = param.DisplayName ?? user.DisplayName,
                Avatar = avatarLink ?? user.Avatar
            };
            return res;
        }
        #endregion

    }


}
