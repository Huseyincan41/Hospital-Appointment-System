using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Entities
{
    public class Appointment:BaseEntity
    {
        
        public DateTime Date { get; set; }
        public int CustomerId { get; set; }
        public string Status { get; set; } = "Bekleyen";
        public Customer Customer { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public int DepartmentId { get; set; } // Yeni alan
        public Department Department { get; set; }
        public TimeSpan Time { get; set; }
    }
}
