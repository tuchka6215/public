using System.Collections.Generic;
using System.Threading.Tasks;
using GroceryStoreDAL.DTO;
using GroceryStoreDAL.Interfaces;

namespace GroceryStoreBL
{
    public class CustomerManager
    {
        public ICustomerRepository customerRepository;
        
        public CustomerManager(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            return await customerRepository.GetCustomers();
        }

        public async Task<Customer> GetCustomer(int customerId)
        {
            return await customerRepository.GetCustomer(customerId);
        }

        public async Task<bool> AddCustomer(Customer customer)
        {
            return await customerRepository.AddCustomer(customer);
        }

        public async Task<bool> UpdateCustomer(Customer customer)
        {
            return await customerRepository.UpdateCustomer(customer);
        }
    }
}
