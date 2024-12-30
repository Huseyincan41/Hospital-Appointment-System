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
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CustomerViewModel>> GetAllCustomersAsync()
        {
            var customers = await _unitOfWork.GetRepository<Customer>().GetAllAsync();

            return customers.Select(c => new CustomerViewModel
            {
                Id = c.Id,
                FullName = $"{c.FullName}",
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                TcNo = c.TcNo
            }).ToList();
        }
        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            var customerRepo = _unitOfWork.GetRepository<Customer>();
            var customer = await customerRepo.GetByIdAsync(customerId);

            if (customer == null)
                return false;

            customerRepo.Delete(customer);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
