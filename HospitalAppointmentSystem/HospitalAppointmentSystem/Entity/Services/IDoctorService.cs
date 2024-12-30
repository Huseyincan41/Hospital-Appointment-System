using Entity.Entities;
using Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Services
{
    public interface IDoctorService
    {
        Task<List<Doctor>> GetDoctorsByDepartmentIdAsync(int departmentId);
        Task AddDoctorAsync(DoctorViewModel viewModel);
        Task<List<DoctorViewModel>> GetAllAsync();
        Task<bool> DeleteDoctorAsync(int doctorId);
        Task<bool> DeleteDoctorWithAppointmentsAsync(int doctorId);
        Task<Doctor> GetDoctorByEmailAsync(string email);
    }
}
