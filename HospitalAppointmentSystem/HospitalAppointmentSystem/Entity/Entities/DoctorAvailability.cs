using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Entities
{
    public class DoctorAvailability:BaseEntity
    {
        
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public DateTime AvailableDate { get; set; }
    }
}
