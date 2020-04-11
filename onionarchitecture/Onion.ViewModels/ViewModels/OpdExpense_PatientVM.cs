using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCO.TFM.WDMS.ViewModels.ViewModels
{
    public class OpdExpense_PatientVM
    {
        public int ID { get; set; }     
        public string NAME { get; set; }
        public int? AGE { get; set; }
        public string RELATIONSHIP_EMPLOYEE { get; set; }
        public int OPDEXPENSE_ID { get; set; }

    }
}
