using Onion.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCO.TFM.WDMS.ViewModels.ViewModels
{
    public class OpdExpensePatientVM
    {
        public int ID { get; set; }     
        public string NAME { get; set; }
        public int? AGE { get; set; }
        public string RELATIONSHIP_EMPLOYEE { get; set; }
        public int OPDEXPENSE_ID { get; set; }

        public virtual OpdExpense OpdExpense { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string Created_By { get; set; }
        public string Modified_By { get; set; }



    }
}
