using Data.Context;
using Entity.Entities;
using Entity.Services;
using Entity.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HospitalDbContext _dbContext;

        public AppointmentService(IUnitOfWork unitOfWork, HospitalDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _unitOfWork.GetRepository<Appointment>()
        .GetAlllAsync(include: a => a.Include(x => x.Customer)
                                    .Include(x => x.Doctor)
                                    .Include(x => x.Department));
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int id)
        {
            return await _unitOfWork.GetRepository<Appointment>().GetByIdAsync(id);
        }

        public async Task AddAppointmentAsync(Appointment appointment)
        {
            await _unitOfWork.GetRepository<Appointment>().Add(appointment);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            _unitOfWork.GetRepository<Appointment>().Update(appointment);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAppointmentAsync(Appointment appointment)
        {
            _unitOfWork.GetRepository<Appointment>().Delete(appointment);
            await _unitOfWork.CommitAsync();
        }
        public async Task DeleteAppointmentAsync(int id)
        {
            var appointment = await _unitOfWork.GetRepository<Appointment>().GetByIdAsync(id);
            if (appointment == null)
            {
                throw new KeyNotFoundException("Randevu bulunamadı.");
            }

            _unitOfWork.GetRepository<Appointment>().Delete(appointment);
            await _unitOfWork.CommitAsync();
        }
        public async Task<List<Appointment>> GetAppointmentsByTcNoAsync(string tcNo)
        {
            return await _unitOfWork.GetRepository<Appointment>()
                .GetAlllAsync(
                    filter: a => a.Customer.TcNo == tcNo,
                    include: a => a.Include(x => x.Customer)
                                  .Include(x => x.Doctor)
                                  .Include(x => x.Department)
                );
        }
        public async Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            var appointment = await _unitOfWork.GetRepository<Appointment>().GetByIdAsync(appointmentId);
            if (appointment == null) return false;
            appointment.Status = "İptal Edildi";
            _unitOfWork.GetRepository<Appointment>().Delete(appointment);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<Appointment> GetAppointmentssByTcNoAsync(string tcNo)
        {
            return await _unitOfWork.GetRepository<Appointment>()
        .GetAsync(
            a => a.Customer.TcNo == tcNo ,
            include: source => source
                .Include(a => a.Customer)
                .Include(a => a.Doctor)
                .Include(a => a.Department)
        );
        }
        public async Task<bool> IsTimeAvailableAsync(int doctorId, DateTime date, TimeSpan time)
        {
            return !await _unitOfWork.GetRepository<Appointment>()
                .AnyAsync(a => a.DoctorId == doctorId && a.Date == date && a.Time == time);
        }
        public List<TimeSpan> GetAvailableTimes()
        {
            var startTime = new TimeSpan(9, 0, 0); // 09:00
            var endTime = new TimeSpan(17, 0, 0); // 17:00
            var interval = new TimeSpan(0, 20, 0); // 20 dakika
            


            var times = new List<TimeSpan>();
            for (var time = startTime; time < endTime; time += interval)
            {
                times.Add(time);
            }

            return times;
        }
        public List<TimeSpan> GetAvailableTimess(DateTime date, int doctorId)
        {
            var startTime = new TimeSpan(9, 0, 0); // 09:00
            var endTime = new TimeSpan(17, 0, 0); // 17:00
            var interval = new TimeSpan(0, 20, 0); // 20 dakika

            var bookedTimes = GetAvailableTimess(date, doctorId);
            var times = new List<TimeSpan>();
            for (var time = startTime; time < endTime; time += interval)
            {
                if (!bookedTimes.Contains(time))
                {
                    times.Add(time);
                }
            }

            return times;
        }

        public async Task<List<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorId)
        {
            //    return await _dbContext.Appointments
            //.Include(a => a.Customer) // Hasta bilgisi
            //.Include(a => a.Department) // Departman bilgisi
            //.Where(a => a.DoctorId == doctorId)
            //.ToListAsync();
            return await _unitOfWork.GetRepository<Appointment>().GetAlllAsync(
       filter: a => a.DoctorId == doctorId && a.Status == "Bekleyen",
       include: query => query.Include(a => a.Customer).Include(a => a.Department)
   );
        }
        public async Task UpdateAppointmentStatusAsync(int appointmentId, string status)
        {
            var appointment = await _unitOfWork.GetRepository<Appointment>().GetByIdAsync(appointmentId);
            if (appointment != null)
            {
                appointment.Status = status;
                _unitOfWork.GetRepository<Appointment>().Update(appointment);
                await _unitOfWork.CommitAsync();
            }
        }
        public async Task<List<TimeSpan>> GetAvailableTimessAsync(DateTime date, int doctorId)
{
    var startTime = new TimeSpan(9, 0, 0); // 09:00
    var endTime = new TimeSpan(17, 0, 0); // 17:00
    var interval = new TimeSpan(0, 20, 0); // 20 dakika

    // Veritabanından belirtilen tarih ve doktor için dolu randevuları al
    var appointments = await _unitOfWork.GetRepository<Appointment>()
        .GetAlllAsync(a => a.Date.Date == date.Date && a.DoctorId == doctorId && a.Status == "Bekleyen");

    // Randevuların saatlerini al
    var bookedTimes = appointments.Select(a => a.Time).ToList();

    // Bütün saatleri oluştur
    var times = new List<TimeSpan>();
    for (var time = startTime; time < endTime; time += interval)
    {
        // Eğer saat dolu değilse listeye ekle
        if (!bookedTimes.Contains(time))
        {
            times.Add(time);
        }
    }

    return times;
}
    }
}
