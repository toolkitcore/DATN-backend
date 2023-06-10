using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Constants
{
    /// <summary>
    /// Chứa các mã lỗi
    /// </summary>
    public static class ErrorCode
    {
        /// <summary>
        /// Too many requests
        /// </summary>
        public const int TooManyRequests = (int)HttpStatusCode.TooManyRequests;

        /// <summary>
        /// Incorrect email or password
        /// </summary>
        public const int Err1000 = 1000;

        /// <summary>
        /// Email already in use
        /// </summary>
        public const int Err1001 = 1001;

        /// <summary>
        /// Invalid account
        /// </summary>
        public const int Err1002 = 1002;

        /// <summary>
        /// Invalid verification token
        /// </summary>
        public const int Err1003 = 1003;

        /// <summary>
        /// Unactivated account
        /// </summary>
        public const int Err1004 = 1004;

        /// <summary>
        /// Dictionary doesn't exist
        /// </summary>
        public const int Err2000 = 2000;

        /// <summary>
        /// Dictionary name already in use
        /// </summary>
        public const int Err2001 = 2001;

        /// <summary>
        /// Dictionary is in use
        /// </summary>
        public const int Err2002 = 2002;

        /// <summary>
        /// Source dictionary is empty
        /// </summary>
        public const int Err2003 = 2003;

        /// <summary>
        /// Concept already exists
        /// </summary>
        public const int Err3001 = 3001;

        /// <summary>
        /// Concept can't be deleted
        /// </summary>
        public const int Err3002 = 3002;

        /// <summary>
        /// A concept can't link to itself
        /// </summary>
        public const int Err3003 = 3003;

        /// <summary>
        /// Circular link
        /// </summary>
        public const int Err3004 = 3004;

        /// <summary>
        /// Concept doesn't exist
        /// </summary>
        public const int Err3005 = 3005;

        /// <summary>
        /// Example doesn't exist
        /// </summary>
        public const int Err4000 = 4000;

        /// <summary>
        /// Duplicate examples
        /// </summary>
        public const int Err4001 = 4001;

        /// <summary>
        /// Invalid parameters
        /// </summary>
        public const int Err9000 = 9000;

        /// <summary>
        /// Invalid file upload
        /// </summary>
        public const int Err9001 = 9001;

        /// <summary>
        /// This file is too large
        /// </summary>
        public const int Err9002 = 9002;

        /// <summary>
        /// This file type is not supported
        /// </summary>
        public const int Err9003 = 9003;

        /// <summary>
        /// Import session does not exist or has expired
        /// </summary>
        public const int Err9004 = 9004;

        /// <summary>
        /// Data is stale
        /// </summary>
        public const int Err9998 = 9998;

        /// <summary>
        /// General error
        /// </summary>
        public const int Err9999 = 9999;
    }

    /// <summary>
    /// Thông báo lỗi
    /// </summary>
    public static class ErrorMessage
    {
        public const string TooManyRequests = "Too many requests";

        public const string Err1000 = "Incorrect email or password";
        public const string Err1001 = "Email already in use";
        public const string Err1002 = "Invalid account";
        public const string Err1003 = "Invalid verification token";
        public const string Err1004 = "Unactivated account";

        public const string Err2000 = "Dictionary doesn't exist";
        public const string Err2001 = "Dictionary name already in use";
        public const string Err2002 = "Dictionary is in use";
        public const string Err2003 = "Source dictionary is empty";

        public const string Err3001 = "Concept already exists";
        public const string Err3002 = "Concept can't be deleted";
        public const string Err3003 = "A concept can't link to itself";
        public const string Err3004 = "Circular link";
        public const string Err3005 = "Concept doesn't exist";

        public const string Err4000 = "Example doesn't exist";
        public const string Err4001 = "Duplicate examples";

        public const string Err9000 = "Invalid parameters";
        public const string Err9001 = "Invalid file upload";
        public const string Err9002 = "This file is too large";
        public const string Err9003 = "This file type is not supported";
        public const string Err9004 = "Import session does not exist or has expired";

        public const string Err9998 = "Data is stale";
    }

    /// <summary>
    /// Thông báo lỗi validate nhập khẩu
    /// </summary>
    public static class ImportValidateErrorMessage
    {
        public const string Required = "{0} cannot be empty";
        public const string NotExist = "{0} does not exists";
        public const string Duplicated = "{0} is duplicated";
        public const string ConceptLinkToItself = ErrorMessage.Err3003;
        public const string ConceptCircleLink = ErrorMessage.Err3004;
    }
}
