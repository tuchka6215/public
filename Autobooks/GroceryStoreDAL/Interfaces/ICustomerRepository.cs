using System.Collections.Generic;
using System.Threading.Tasks;
using GroceryStoreDAL.DTO;

namespace GroceryStoreDAL.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetCustomers();
        Task<Customer> GetCustomer(int customerId);
        Task<bool> AddCustomer(Customer customer);
        Task<bool> UpdateCustomer(Customer customer);
        Task<bool> RemoveCustomer(int customerId);
    }
}
