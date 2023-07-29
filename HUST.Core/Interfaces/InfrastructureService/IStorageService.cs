using HUST.Core.Models.Param;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.InfrastructureService
{
    public interface IStorageService
    {
        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<string> UploadAsync(string folderPath, string fileName, byte[] file);

        /// <summary>
        /// Get file url
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<string> GetDownloadUrlAsync(string folderPath, string fileName);

        /// <summary>
        /// Xóa file
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(string folderPath, string fileName);
    }
}
