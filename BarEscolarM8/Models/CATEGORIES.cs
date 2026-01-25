using System.Collections.Generic;

namespace APiConsumer.Models
{
    public class CATEGORIES
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // opcional: lista de produtos (pode ser nula quando não carregada)
        public List<PRODUCTS> products { get; set; }
    }
}