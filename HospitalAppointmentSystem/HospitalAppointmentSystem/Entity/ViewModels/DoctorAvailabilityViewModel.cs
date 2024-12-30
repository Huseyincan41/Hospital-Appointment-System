using Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.ViewModels
{
    public class DoctorAvailabilityViewModel
    {
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public DateTime AvailableDate { get; set; }
    }
}
