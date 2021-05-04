using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GroceryStoreDAL;
using GroceryStoreDAL.DTO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GroceryStoreUnitTests
{
    [TestClass]
    public class CustomerRepositoryUnitTests : UnitTestsBase
    {
        private CustomerRepository repository;

        public CustomerRepositoryUnitTests()
        {
            string dbFileName = this.configuration.GetValue<string>("DatabaseFileName");
            CustomerRepository.Init(dbFileName).Wait();
            this.repository = new CustomerRepository();
        }

        [TestMethod]
        public async Task GetCustomers()
        {
            var customers = await repository.GetCustomers();
            
            Assert.IsNotNull(customers);
            Assert.IsTrue(customers.FirstOrDefault() != null);
        }

        [TestMethod]
        public async Task GetCustomer()
        {
            var customers = await repository.GetCustomers();
            var customer = customers.FirstOrDefault();

            var customer2 = await repository.GetCustomer(customer.id);

            Assert.IsNotNull(customer2);
            Assert.AreEqual(customer.id, customer2.id);
            Assert.AreEqual(customer.name, customer2.name);
        }

        [TestMethod]
        public async Task GetCustomer_NotFound()
        {
            var customer = await repository.GetCustomer(-1);

            Assert.IsNull(customer);
        }

        [TestMethod]
        public async Task AddCustomer()
        {
            var customer = new Customer() { id = int.MaxValue, name = Guid.NewGuid().ToString() };

            var added = await repository.AddCustomer(customer);
            Assert.IsTrue(added);

            var customer2 = await repository.GetCustomer(customer.id);
            Assert.IsNotNull(customer2);
            Assert.AreEqual(customer.id, customer2.id);
            Assert.AreEqual(customer.name, customer2.name);

            await repository.RemoveCustomer(customer.id);
        }

        [TestMethod]
        public async Task AddCustomer_Duplicate()
        {
            var customer = new Customer() { id = int.MaxValue - 1, name = Guid.NewGuid().ToString() };

            var added = await repository.AddCustomer(customer);
            Assert.IsTrue(added);

            var added2 = await repository.AddCustomer(new Customer() { id = customer.id, name = Guid.NewGuid().ToString() });
            Assert.IsFalse(added2);

            var customer2 = await repository.GetCustomer(customer.id);
            Assert.IsNotNull(customer2);
            Assert.AreEqual(customer.id, customer2.id);
            Assert.AreEqual(customer.name, customer2.name);

            await repository.RemoveCustomer(customer.id);
        }

        [TestMethod]
        public async Task UpdateCustomer()
        {
            var customer = new Customer() { id = int.MaxValue - 2, name = Guid.NewGuid().ToString() };
            var added = await repository.AddCustomer(customer);
            var customer2 = await repository.GetCustomer(customer.id);

            string randomNewName = Guid.NewGuid().ToString();
            customer2.name = randomNewName;
            bool updated = await repository.UpdateCustomer(customer2);
            Assert.IsTrue(updated);

            var customer3 = await repository.GetCustomer(customer.id);
            Assert.IsNotNull(customer3);
            Assert.AreEqual(customer.id, customer3.id);
            Assert.AreEqual(randomNewName, customer3.name);

            await repository.RemoveCustomer(customer.id);
        }

        [TestMethod]
        public async Task UpdateCustomer_NotFound()
        {
            string randomNewName = Guid.NewGuid().ToString();
            var customer = new Customer() { id = -1, name = randomNewName };
            bool updated = await repository.UpdateCustomer(customer);

            Assert.IsFalse(updated);
        }
    }
}
