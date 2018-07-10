using System.Collections.Generic;

namespace EmitMapper.Benchmarks.TestData.Automapper
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Credit { get; set; }
        public Address Address { get; set; }
        public Address HomeAddress { get; set; }
        public Address[] Addresses { get; set; }
        public List<Address> WorkAddresses { get; set; }
    }
}