using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Entities
{
    public class Department:BaseEntity
    {
        
        public string Name { get; set; }
        public string Description { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public ICollection<Doctor> Doctors { get; set; }
        
    }
}
