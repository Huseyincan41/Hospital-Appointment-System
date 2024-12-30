using Entity.Entities;
using Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetDepartmentsWithDoctorsAsync();
        Task<List<Department>> GetAllAsync();
        Task<List<DepartmentViewModel>> GetAllDepartmentsAsync();
        Task AddDepartmentAsync(DepartmentViewModel viewModel);
        Task<bool> DeleteDepartmentAsync(int departmentId);
    }
}
