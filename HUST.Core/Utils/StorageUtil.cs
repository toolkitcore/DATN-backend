using Firebase.Auth;
using Firebase.Storage;
using HUST.Core.Constants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HUST.Core.Utils
{
    public class StorageUtil
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private string FirebaseToken { get; set; }
        private string Email { get; set; }
        private string Password { get; set; }
        private string APIKey { get; set; }

        private string Storage { get; set; }

        public StorageUtil(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
            Email = _configuration[AppSettingKey.FirebaseConfigs.Email];
            Password = _configuration[AppSettingKey.FirebaseConfigs.Password];
            APIKey = _configuration[AppSettingKey.FirebaseConfigs.APIKey];
            Storage = _configuration[AppSettingKey.FirebaseConfigs.Storage];
        }

        /// <summary>
        /// Lấy token
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetFirebaseToken()
        {
            if(!string.IsNullOrEmpty(FirebaseToken))
            {
                return this.FirebaseToken;
            }
            var auth = new FirebaseAuthProvider(new FirebaseConfig(APIKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(Email, Password);
            FirebaseToken = a.FirebaseToken;
            return FirebaseToken;
           
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<string> UploadAsync(string folderPath, string fileName, byte[] file)
        {
            var stream = new MemoryStream(file);
            var token = await GetFirebaseToken();
            var cancellation = new CancellationTokenSource();
            var task = new FirebaseStorage(
                Storage,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(token),
                    ThrowOnCancel = true
                })
                .Child(_environment.EnvironmentName)
                .Child(folderPath)
                .Child(fileName)
                .PutAsync(stream, cancellation.Token);

            // Track progress of the upload
            //task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            var downloadUrl = await task;
            return downloadUrl;
        }

        /// <summary>
        /// Get file url
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<string> GetDownloadUrlAsync(string folderPath, string fileName)
        {
            var token = await GetFirebaseToken();
            var task = new FirebaseStorage(
                Storage,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(token),
                    ThrowOnCancel = true
                })
                .Child(_environment.EnvironmentName)
                .Child(folderPath)
                .Child(fileName)
                .GetDownloadUrlAsync();

            // Track progress of the upload
            //task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            var downloadUrl = await task;
            return downloadUrl;
        }

        /// <summary>
        /// Xóa file
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string folderPath, string fileName)
        {
            var token = await GetFirebaseToken();
            var task = new FirebaseStorage(
                Storage,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(token),
                    ThrowOnCancel = true
                })
                .Child(_environment.EnvironmentName)
                .Child(folderPath)
                .Child(fileName)
                .DeleteAsync();

            // Track progress of the upload
            //task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");
            await task;
            return true;
        }

    }
}
