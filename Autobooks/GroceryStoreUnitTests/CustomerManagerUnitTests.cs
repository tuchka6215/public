using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GroceryStoreBL;
using System.Threading.Tasks;
using GroceryStoreDAL.DTO;
using GroceryStoreDAL.Interfaces;

namespace GroceryStoreUnitTests
{
    [TestClass]
    public class CustomerManagerUnitTests
    {
        private ICustomerRepository repository;
        private CustomerManager manager;

        public CustomerManagerUnitTests()
        {
            this.repository = new CustomerRepositoryMock();
            this.manager = new CustomerManager(repository);
        }

        [TestMethod]
        public async Task GetCustomers()
        {
            var customers = await manager.GetCustomers();

            Assert.IsNotNull(customers);
            Assert.AreEqual(customers.Count(), (await repository.GetCustomers()).Count());
        }

        [TestMethod]
        public async Task GetCustomer()
        {
            var customer = (await repository.GetCustomers()).First();

            var customer2 = await manager.GetCustomer(customer.id);

            Assert.IsNotNull(customer2);
            Assert.AreEqual(customer.id, customer2.id);
            Assert.AreEqual(customer.name, customer2.name);
        }

        [TestMethod]
        public async Task GetCustomer_NotFound()
        {
            var customer = await manager.GetCustomer(-1);

            Assert.IsNull(customer);
        }

        [TestMethod]
        public async Task AddCustomer()
        {
            var customer = new Customer() { id = int.MaxValue, name = Guid.NewGuid().ToString() };

            var added = await manager.AddCustomer(customer);
            Assert.IsTrue(added);

            var customer2 = await manager.GetCustomer(customer.id);
            Assert.IsNotNull(customer2);
            Assert.AreEqual(customer.id, customer2.id);
            Assert.AreEqual(customer.name, customer2.name);
        }

        [TestMethod]
        public async Task AddCustomer_Duplicate()
        {
            var customer = new Customer() { id = int.MaxValue - 1, name = Guid.NewGuid().ToString() };

            var added = await manager.AddCustomer(customer);
            Assert.IsTrue(added);

            var added2 = await manager.AddCustomer(new Customer() { id = customer.id, name = Guid.NewGuid().ToString() });
            Assert.IsFalse(added2);

            var customer2 = await manager.GetCustomer(customer.id);
            Assert.IsNotNull(customer2);
            Assert.AreEqual(customer.id, customer2.id);
            Assert.AreEqual(customer.name, customer2.name);
        }

        [TestMethod]
        public async Task UpdateCustomer()
        {
            var customer = new Customer() { id = int.MaxValue - 2, name = Guid.NewGuid().ToString() };
            var added = await manager.AddCustomer(customer);
            var customer2 = await manager.GetCustomer(customer.id);

            string randomNewName = Guid.NewGuid().ToString();
            customer2.name = randomNewName;
            bool updated = await manager.UpdateCustomer(customer2);
            Assert.IsTrue(updated);

            var customer3 = await manager.GetCustomer(customer.id);
            Assert.IsNotNull(customer3);
            Assert.AreEqual(customer.id, customer3.id);
            Assert.AreEqual(randomNewName, customer3.name);
        }

        [TestMethod]
        public async Task UpdateCustomer_NotFound()
        {
            string randomNewName = Guid.NewGuid().ToString();
            var customer = new Customer() { id = -1, name = randomNewName };
            bool updated = await manager.UpdateCustomer(customer);

            Assert.IsFalse(updated);
        }
    }
}
