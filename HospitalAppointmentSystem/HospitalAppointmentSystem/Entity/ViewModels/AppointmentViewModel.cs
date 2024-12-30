using Entity.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.ViewModels
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public Customer Customer { get; set; }
        
        public Doctor Doctor { get; set; }
        
        public Department Department { get; set; }
        public List<Department> Departments { get; set; } = new ();
        public List<Doctor> Doctors { get; set; } = new ();
        public string CustomerName { get; set; } // Müşteri Adı
        public string CustomerTc { get; set; } // Müşteri Adı
        public string DoctorName { get; set; } // Doktor Adı
        public string DepartmentName { get; set; } // Departman Adı
        public TimeSpan Time { get; set; }
        public string FormattedTime => Time.ToString(@"hh\:mm");
        public string Status { get; set; } = "Bekleyen";

    }
}
