using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Entities
{
    public class Doctor:BaseEntity
    {
        
        public string FullName { get; set; }
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public string ImageUrl { get; set; }= "";
        public ICollection<Appointment> Appointments { get; set; }

    }
}
