using Data.Identity;
using Entity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Data.Context
{
    public class HospitalDbContext:IdentityDbContext<AppUser,AppRole,int>
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) :base(options) { }
        
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict); // Cascade yerine Restrict

            //        modelBuilder.Entity<Appointment>()
            //.HasOne(a => a.Doctor)
            //.WithMany(d => d.Appointments)
            //.HasForeignKey(a => a.DoctorId)
            //.OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict); // Cascade yerine Restrict

            //modelBuilder.Entity<Appointment>()
            //    .HasOne(a => a.Doctor)
            //    .WithMany()
            //    .HasForeignKey(a => a.DoctorId)
            //    .OnDelete(DeleteBehavior.SetNull);

            //modelBuilder.Entity<Appointment>()
            //   .HasOne(a => a.Doctor)
            //   .WithMany()
            //   .HasForeignKey(a => a.DoctorId)
            //   .OnDelete(DeleteBehavior.Cascade);

            //    modelBuilder.Entity<Appointment>()
            //.HasOne(a => a.Doctor)
            //.WithMany()
            //.HasForeignKey(a => a.DoctorId)
            //.OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Department>().HasData(
                new Department { Id=1,Name = "Kardiyoloji" },
                new Department { Id = 2, Name = "Dahiliye" }
                    );

            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { Id = 1, FullName = "Dr. Ahmet Yılmaz", DepartmentId = 1 },
                new Doctor { Id = 2, FullName = "Dr. Ayşe Demir", DepartmentId = 2 }
                    );

            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, FullName = "Mehmet Kaya", Email = "mehmet.kaya@example.com", PhoneNumber = "5551234567",TcNo="1213321321" },
                new Customer { Id = 2, FullName = "Zeynep Arslan", Email = "zeynep.arslan@example.com", PhoneNumber = "5559876543",TcNo = "1213321321" }
                    );

            modelBuilder.Entity<DoctorAvailability>().HasData(
                new DoctorAvailability { Id = 1, DoctorId = 1, AvailableDate = new DateTime(2024, 12, 10) },
                new DoctorAvailability { Id = 2, DoctorId = 2, AvailableDate = new DateTime(2024, 12, 11) }
                    );
        }
    }
}
