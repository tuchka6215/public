using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using GroceryStoreBL;
using GroceryStoreDAL;
using GroceryStoreDAL.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GroceryStoreDAL.Interfaces;

namespace GroceryStoreAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private IConfiguration configuration;
        private ILogger<CustomersController> logger;
        private CustomerManager manager;

        public CustomersController(IConfiguration configuration, ILogger<CustomersController> logger, ICustomerRepository repository)
        {
            this.configuration = configuration;
            this.logger = logger;

            var dbFileName = configuration.GetValue<string>("DatabaseFileName");
            CustomerRepository.Init(dbFileName).Wait();
            this.manager = new CustomerManager(repository);

            //throw new System.Exception("Test global exception handling");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> Get()
        {
            var customers = await manager.GetCustomers();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> Get(int id)
        {
            var customer = await manager.GetCustomer(id);
            
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Customer customer)
        {
            var added = await manager.AddCustomer(customer);
            if (!added)
                return BadRequest();

            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] Customer customer)
        {
            bool updated = await manager.UpdateCustomer(customer);
            if (!updated)
                return NotFound();

            return Ok();
        }
    }
}
