using System.Collections.Generic;

namespace APiConsumer.Models
{
    public class ORDERS
    {
        public int id { get; set; }
        public string userid { get; set; }
        public decimal? total { get; set; }
        public string createdat { get; set; } // ou DateTime se preferir
        public bool? state { get; set; }
        public string type { get; set; }

        public List<ORDERITEMS> orderitems { get; set; }
    }
}