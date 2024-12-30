using Entity.Entities;
using Entity.Services;
using Entity.UnitOfWorks;
using Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class DepartmentService:IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Department>> GetDepartmentsWithDoctorsAsync()
        {
            return await _unitOfWork.GetRepository<Department>().GetAllAsync(
                includeProperties: "Doctors"
            );
        }
        public async Task<List<Department>> GetAllAsync()
        {
            return (await _unitOfWork.GetRepository<Department>().GetAllAsync()).ToList();
        }
        public async Task AddDepartmentAsync(DepartmentViewModel viewModel)
        {
            var department = new Department
            {
                Name = viewModel.Name,
                ImageUrl = viewModel.ImageUrl,
                Description = viewModel.Description,
            };

            await _unitOfWork.GetRepository<Department>().Add(department);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> DeleteDepartmentAsync(int departmentId)
        {
            var department = await _unitOfWork.GetRepository<Department>().GetByIdAsync(departmentId);
            if (department == null)
                return false;

            _unitOfWork.GetRepository<Department>().Delete(department);
            await _unitOfWork.CommitAsync();
            return true; 
        }
        public async Task<List<DepartmentViewModel>> GetAllDepartmentsAsync()
        {
            var departments = await _unitOfWork.GetRepository<Department>().GetAllAsync();
            return departments.Select(d => new DepartmentViewModel
            {
                Id = d.Id,
                Name = d.Name,
                ImageUrl = d.ImageUrl,
                Description = d.Description,
            }).ToList();
        }
    }
}
