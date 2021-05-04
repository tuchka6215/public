using System.IO;
using Microsoft.Extensions.Configuration;
using GroceryStoreDAL;

namespace GroceryStoreUnitTests
{
    public class UnitTestsBase
    {
        protected IConfiguration configuration;
        
        public UnitTestsBase()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            this.configuration = builder.Build();
        }
    }
}
