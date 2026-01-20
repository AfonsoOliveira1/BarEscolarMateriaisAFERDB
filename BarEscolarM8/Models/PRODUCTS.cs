using System.Collections.Generic;

namespace APiConsumer.Models
{
    public class PRODUCTS
    {
        public decimal? price { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int? categoryid { get; set; }
        public int? kcal { get; set; }
        public int? protein { get; set; }
        public int? fat { get; set; }
        public int? carbs { get; set; }
        public int? salt { get; set; }
        public string allergens { get; set; }
        public int? stock { get; set; }
        public bool? isactive { get; set; }

        // opcional: navegação curta
        public CATEGORIES category { get; set; }
        public List<ORDERITEMS> orderitems { get; set; }
    }
}