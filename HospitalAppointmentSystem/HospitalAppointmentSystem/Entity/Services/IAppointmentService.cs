using Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment> GetAppointmentByIdAsync(int id);
        Task AddAppointmentAsync(Appointment appointment);
        Task UpdateAppointmentAsync(Appointment appointment);
        Task DeleteAppointmentAsync(Appointment appointment);
        Task DeleteAppointmentAsync(int id);
        Task<List<Appointment>> GetAppointmentsByTcNoAsync(string tcNo);
        Task<bool> CancelAppointmentAsync(int appointmentId);
        Task<Appointment> GetAppointmentssByTcNoAsync(string tcNo);
        Task<bool> IsTimeAvailableAsync(int doctorId, DateTime date, TimeSpan time);
        List<TimeSpan> GetAvailableTimes();
        List<TimeSpan> GetAvailableTimess(DateTime date, int doctorId);
        Task<List<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorId);
        Task UpdateAppointmentStatusAsync(int appointmentId, string status);
        Task<List<TimeSpan>> GetAvailableTimessAsync(DateTime date, int doctorId);
    }
}
