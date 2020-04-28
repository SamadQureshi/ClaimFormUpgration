using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Common.Utils
{
    public class EmailMessage
    {
        public IEnumerable<string> To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public IEnumerable<string> Cc { get; set; }
        public IEnumerable<string> Bcc { get; set; }
        public IEnumerable<string> Attachments { get; set; }
        public bool IsHtml { get; set; }
    }
}
