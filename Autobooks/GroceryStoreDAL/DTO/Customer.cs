using System;

namespace GroceryStoreDAL.DTO
{
    public class Customer
    {
        public int id { get; set; }
        public string name { get; set; }

        public Customer Clone()
        {
            return new Customer() { id = this.id, name = this.name };
        }
    }
}
