using System;

namespace Onion.Domain.Models
{
    public class BaseEntity
    {
        
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string Created_By { get; set; }
        public string Modified_By { get; set; }


    }
}
