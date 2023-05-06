using System;

namespace HUST.Core.Exceptions
{
    /// <summary>
    /// Lớp base cho exception trong chương trình
    /// </summary>
    public class HustException : Exception
    {
        public HustException() : base()
        {

        }

        public HustException(string msg) : base(msg)
        {

        }
    }
}
