using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Settings
{
    public class SendgridMailSettings
    {
        public string SendGridKey { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }

    }
}
