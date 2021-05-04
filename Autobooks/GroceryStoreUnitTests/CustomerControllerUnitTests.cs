using GroceryStoreAPI;
using GroceryStoreDAL.DTO;
using GroceryStoreDAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryStoreUnitTests
{
    [TestClass]
    public class CustomerControllerUnitTests : UnitTestsBase
    {
        private CustomersController controller;
        private ICustomerRepository repository;
        
        public CustomerControllerUnitTests()
        {
            this.repository = new CustomerRepositoryMock();
            this.controller = new CustomersController(configuration, new NullLogger<CustomersController>(), repository);
        }

        [TestMethod]
        public async Task GetCustomers() 
        {
            var result = await this.controller.Get();
            
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            var customers = okResult.Value as IEnumerable<Customer>;
            Assert.IsNotNull(customers);

            Assert.AreEqual(customers.Count(), (await repository.GetCustomers()).Count());
        }

        [TestMethod]
        public async Task GetCustomer()
        {
            var customer = (await repository.GetCustomers()).First();
            
            var result = await this.controller.Get(customer.id);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            var customer2 = okResult.Value as Customer;

            Assert.IsNotNull(customer2);
            Assert.AreEqual(customer.id, customer2.id);
            Assert.AreEqual(customer.name, customer2.name);
        }

        [TestMethod]
        public async Task GetCustomer_NotFound()
        {
            var result = await this.controller.Get(-1);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task AddCustomer()
        {
            var customer = new Customer() { id = int.MaxValue, name = Guid.NewGuid().ToString() };

            var result = await controller.Post(customer);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));

            var customer2 = await repository.GetCustomer(customer.id);
            Assert.IsNotNull(customer2);
            Assert.AreEqual(customer.id, customer2.id);
            Assert.AreEqual(customer.name, customer2.name);
        }

        [TestMethod]
        public async Task AddCustomer_Duplicate()
        {
            var customer = new Customer() { id = int.MaxValue - 1, name = Guid.NewGuid().ToString() };

            var result = await controller.Post(customer);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));

            var result2 = await controller.Post(new Customer() { id = customer.id, name = Guid.NewGuid().ToString() });
            Assert.IsNotNull(result2);
            Assert.IsInstanceOfType(result2, typeof(BadRequestResult));

            var customer2 = await repository.GetCustomer(customer.id);
            Assert.IsNotNull(customer2);
            Assert.AreEqual(customer.id, customer2.id);
            Assert.AreEqual(customer.name, customer2.name);
        }

        [TestMethod]
        public async Task UpdateCustomer()
        {
            var customer = new Customer() { id = int.MaxValue - 2, name = Guid.NewGuid().ToString() };
            var added = await repository.AddCustomer(customer);
            var customer2 = await repository.GetCustomer(customer.id);

            string randomNewName = Guid.NewGuid().ToString();
            customer2.name = randomNewName;
            
            var result = await controller.Put(customer2);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));

            var customer3 = await repository.GetCustomer(customer.id);
            Assert.IsNotNull(customer3);
            Assert.AreEqual(customer.id, customer3.id);
            Assert.AreEqual(randomNewName, customer3.name);
        }

        [TestMethod]
        public async Task UpdateCustomer_NotFound()
        {
            string randomNewName = Guid.NewGuid().ToString();
            var customer = new Customer() { id = -1, name = randomNewName };
            
            var result = await controller.Put(customer);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
