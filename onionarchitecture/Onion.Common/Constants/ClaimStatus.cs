using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Common.Constants
{
    public static class ClaimStatus
    {
        public const string INPROGRESS = "InProcess";
        public const string SUBMITTED = "Submitted";

        public const string HRAPPROVED = "HR-Approved";
        public const string HRREJECTED = "HR-Rejected";
        public const string HRINPROCESS = "HR-InProcess";


        public const string FINAPPROVED = "FIN-Approved";
        public const string FINREJECTED = "FIN-Rejected";
        public const string FININPROCESS = "FIN-InProcess";


        public const string MANAPPROVED = "MAN-Approved";
        public const string MANREJECTED = "MAN-Rejected";
        public const string MANINPROCESS = "MAN-InProcess";

        public const string MANGINPROCESS = "MANG-InProcess";
        public const string MANGAPPROVED = "MANG-Approved";
        public const string MANGREJECT = "MANG-Reject";

        public const string COMPLETED = "Completed";

    }
}
