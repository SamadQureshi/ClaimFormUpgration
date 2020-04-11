using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCO.TFM.WDMS.ViewModels.ViewModels
{
    public class OpdExpense_ImageVM
    {
        public int IMAGE_ID { get; set; }

        public int? OPDEXPENSE_ID { get; set; }

        public string IMAGE_NAME { get; set; }

        public string IMAGE_EXT { get; set; }

        public string IMAGE_BASE64 { get; set; }

        public string NAME_EXPENSES { get; set; }

        public decimal? EXPENSE_AMOUNT { get; set; }
    }
}
