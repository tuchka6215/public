using GroceryStoreDAL.DTO;
using GroceryStoreDAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryStoreUnitTests
{
    class CustomerRepositoryMock : ICustomerRepository
    {
        private List<Customer> customers = new List<Customer>() 
        { 
            new Customer() { id = 1, name = "Name1" }, 
            new Customer() { id = 2, name = "Name2" } 
        };

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            lock (customers)
            {
                return customers;
            }
        }
        
        public async Task<Customer> GetCustomer(int customerId)
        {
            lock (customers) 
            { 
                return customers.FirstOrDefault(x => x.id == customerId); 
            }
        }
        
        public async Task<bool> AddCustomer(Customer customer)
        {
            lock (customers)
            {
                var customerDb = customers.FirstOrDefault(x => x.id == customer.id);
                if (customerDb != null)
                    return false;
                customers.Add(customer.Clone());
                return true;
            }
        }
        
        public async Task<bool> UpdateCustomer(Customer customer) 
        {
            lock (customers)
            {
                var customerDb = customers.FirstOrDefault(x => x.id == customer.id);
                if (customerDb == null)
                    return false;
                customers.Remove(customerDb);
                customers.Add(customer.Clone());
                return true;
            }
        }

        public async Task<bool> RemoveCustomer(int customerId)
        {
            lock (customers)
            {
                var customerDb = customers.FirstOrDefault(x => x.id == customerId);
                if (customerDb == null)
                    return false;
                customers.Remove(customerDb);
                return true;
            }
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
