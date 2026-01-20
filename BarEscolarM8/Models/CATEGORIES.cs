using System.Collections.Generic;

namespace APiConsumer.Models
{
    public class CATEGORIES
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        // opcional: lista de produtos (pode ser nula quando não carregada)
        public List<PRODUCTS> products { get; set; }
    }
}