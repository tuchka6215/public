using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GroceryStoreDAL.DTO;
using GroceryStoreDAL.Interfaces;

namespace GroceryStoreDAL
{
    public class CustomerRepository : ICustomerRepository
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            AllowTrailingCommas = true,
        };

        private static string DbFilePath = null;

        //private static object databaseLock = new object();
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        private static CustomerDB Db = null;

        public static async Task Init(string databaseFileName)
        {
            await semaphore.WaitAsync();
            try
            {
                //lock (databaseLock) 
                if (DbFilePath == null)
                {
                    //lock (databaseLock)
                    {
                        if (DbFilePath == null)
                        {
                            DbFilePath = Directory.GetCurrentDirectory() + "/" + databaseFileName;
                            await ReadDB();
                        }
                    }
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private static async Task ReadDB()
        {
            //var jsonText = await System.IO.File.ReadAllTextAsync(DbFilePath);
            using (var stream = File.OpenRead(DbFilePath))
                Db = await JsonSerializer.DeserializeAsync<CustomerDB>(stream, JsonSerializerOptions);
        }

        private static async Task SaveDB()
        {
            using (var stream = File.Create(DbFilePath))
                await JsonSerializer.SerializeAsync<CustomerDB>(stream, Db, JsonSerializerOptions);
            //File.WriteAllText(DbFilePath, jsonText);
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            await semaphore.WaitAsync();
            try
            {
                return Db.customers.Select(x => x.Clone());
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<Customer> GetCustomer(int customerId)
        {
            await semaphore.WaitAsync();
            try
            {
                var customer = Db.customers.FirstOrDefault(x => x.id == customerId);
                if (customer == null)
                    return null;
                return customer.Clone();
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<bool> AddCustomer(Customer customer)
        {
            await semaphore.WaitAsync();
            try
            {
                var customer2 = Db.customers.FirstOrDefault(x => x.id == customer.id);
                if (customer2 != null)
                    return false;
                var customerNew = customer.Clone();
                Db.customers.Add(customerNew);
                await SaveDB();
                return true;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<bool> UpdateCustomer(Customer customer)
        {
            await semaphore.WaitAsync();
            try
            {
                var customerDb = Db.customers.FirstOrDefault(x => x.id == customer.id);
                if (customerDb == null)
                    return false;
                Db.customers.Remove(customerDb);
                Db.customers.Add(customer.Clone());
                await SaveDB();
                return true;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<bool> RemoveCustomer(int customerId)
        {
            await semaphore.WaitAsync();
            try
            {
                var customerDb = Db.customers.FirstOrDefault(x => x.id == customerId);
                if (customerDb == null)
                    return false;
                Db.customers.Remove(customerDb);
                await SaveDB();
                return true;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
