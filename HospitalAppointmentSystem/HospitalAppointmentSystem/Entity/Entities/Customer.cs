using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Entities
{
    public class Customer:BaseEntity
    {
        
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string TcNo { get; set; } = "";
        public ICollection<Appointment> Appointments { get; set; }
    }
}
