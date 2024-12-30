using Data.Context;
using Entity.Entities;
using Entity.Services;
using Entity.UnitOfWorks;
using Entity.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HospitalDbContext _context;

        public DoctorService(IUnitOfWork unitOfWork, HospitalDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<List<Doctor>> GetDoctorsByDepartmentIdAsync(int departmentId)
        {
            return (await _unitOfWork.GetRepository<Doctor>()
                .GetAllAsync(d => d.DepartmentId == departmentId))
                .ToList();
        }
        public async Task AddDoctorAsync(DoctorViewModel viewModel)
        {
            var doctor = new Doctor
            {
                FullName = viewModel.FullName,
                DepartmentId = viewModel.DepartmentId,
                ImageUrl = viewModel.ImageUrl,
                Email = viewModel.Email,
                Password = viewModel.Password,
            };

            await _unitOfWork.GetRepository<Doctor>().Add(doctor);
            await _unitOfWork.CommitAsync();
        }
        public async Task<List<DoctorViewModel>> GetAllAsync()
        {
            var doctors = await _unitOfWork.GetRepository<Doctor>()
                                           .GetAlllAsync(include: d => d.Include(x => x.Department));

            return doctors.Select(doctor => new DoctorViewModel
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                DepartmentId = doctor.DepartmentId,
                 DepartmentName=doctor.Department.Name,
                ImageUrl = doctor.ImageUrl,
                Email = doctor.Email,
                Password = doctor.Password,
              
            }).ToList();
        }

        public async Task<bool> DeleteDoctorAsync(int doctorId)
        {
            // Doktora bağlı randevuları kontrol et
            var hasAppointments = await _unitOfWork.GetRepository<Appointment>()
                                           .AnyAsync(x => x.DoctorId == doctorId);

            if (hasAppointments)
                return false; // Randevular varsa silmeye izin verme

            // Doktoru getir ve sil
            var doctor = await _unitOfWork.GetRepository<Doctor>().GetByIdAsync(doctorId);
            if (doctor == null)
                return false;

            _unitOfWork.GetRepository<Doctor>().Delete(doctor);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<bool> DeleteDoctorWithAppointmentsAsync(int doctorId)
        {
            var appointments = await _unitOfWork.GetRepository<Appointment>()
                                       .GetAllAsync(a => a.DoctorId == doctorId);

            if (appointments != null && appointments.Any())
            {
                // Tüm randevuları sil
                foreach (var appointment in appointments)
                {
                    _unitOfWork.GetRepository<Appointment>().Delete(appointment);
                }
            }

            // Doktoru getir ve sil
            var doctor = await _unitOfWork.GetRepository<Doctor>().GetByIdAsync(doctorId);
            if (doctor == null)
                return false;

            _unitOfWork.GetRepository<Doctor>().Delete(doctor);

            // Tüm değişiklikleri kaydet
            await _unitOfWork.CommitAsync();
            return true;
        }
        public async Task<Doctor> GetDoctorByEmailAsync(string email)
        {
            return await _context.Doctors.FirstOrDefaultAsync(d => d.Email == email);
        }
    }
  }

