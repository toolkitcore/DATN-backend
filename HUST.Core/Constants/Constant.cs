using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Constants
{
    /// <summary>
    /// Hằng giá trị
    /// </summary>
    public static class Constant
    {
       
    }

    /// <summary>
    /// Giá trị xác định trước độ mạnh của liên kết
    /// </summary>
    public static class LinkStrength
    {
        public const int TwoPrimary = 2;
        public const int PrimaryAndAssociatedNonPrimary = 2;
        public const int TwoAssociatedNonPrimary = 1;
        public const int PrimaryAndUnAssociatedNonPrimary = 0;
        public const int TwoUnassociatedNonPrimary = -1;
        public const int Itself = 1;
    }

}
